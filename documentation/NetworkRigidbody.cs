using UnityEngine;
using System.Collections;

public class NetworkRigidbody : MonoBehaviour {
	
        public double interpolationBackTime = 0.2; 
	public double extrapolationLimit = 0.5; 
        
        internal struct  State {
                internal double timestamp;
                internal Vector3 pos; 
		internal Vector3 velocity;
        }	
        // We store twenty states with "playback" information
        State[] m_BufferedState = new State[20]; // TODO: interpolationBackTime/sendRate is enough
	
        // Keep track of what slots are used
        int m_TimestampCount;	
	
	// Cubic hermite interpolation
	Vector3 interpolate(Vector3 p0, Vector3 p1, Vector3 v0, Vector3 v1, float t) {	
		float t2 = t * t;
		float t3 = t2 * t;
		
		float H0 = 1.0f - 3.0f * t2 + 2.0f * t3;
		float H1 = t    - 2.0f * t2 +        t3;
		float H2 =      -        t2 +        t3;
		float H3 = 	  3.0f * t2 - 2.0f * t3;	   
		
		return H0 * p0 + H1 * v0 + H2 * v1 + H3 * p1;
	}
	
	void FixedUpdate() {     	
                double currentTime = Network.time;
                double interpolationTime = currentTime - interpolationBackTime;

                if (interpolationTime < m_BufferedState[0].timestamp) {
                        for (int i = 0; i < m_TimestampCount; i++) {
                                if (m_BufferedState[i].timestamp <= interpolationTime || (i == m_TimestampCount - 1)) {
                                        State rhs = m_BufferedState[Mathf.Max(i - 1, 0)];
                                        State lhs = m_BufferedState[i];
                                        
                                        double length = rhs.timestamp - lhs.timestamp;
                                        float t = 0.0f;
					double timeDiff = (interpolationTime - lhs.timestamp);
                                        if (length > 0.0001)
                                                t = (float)(timeDiff / length);	
					
					rigidbody.position = interpolate(lhs.pos, rhs.pos, (float)length * lhs.velocity, (float)length * rhs.velocity, t);
					
                                        return;
                                }
                        }
                }
                else {	
  	    		//Debug.Log("!!! Using extrapolation");
			double extrapolation = interpolationTime - m_BufferedState[0].timestamp;
			if (extrapolation <= extrapolationLimit) {
				rigidbody.position = m_BufferedState[0].pos + m_BufferedState[0].velocity * (float)extrapolation;
			}
                }
        }	
	
	// TODO only send 2D data
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Vector3 s_position = transform.localPosition;
		Vector3 s_velocity = rigidbody.velocity;
		if (stream.isWriting) {
			stream.Serialize(ref s_position);
			stream.Serialize(ref s_velocity);
		} else {
			stream.Serialize(ref s_position);
			stream.Serialize(ref s_velocity);
			
                        for (int i = m_BufferedState.Length-1; i >= 1; i--) {
                                m_BufferedState[i] = m_BufferedState[i-1];
                        }
                        
                        // Save currect received state as 0 in the buffer, safe to overwrite after shifting
                        State state;
                        state.timestamp = Network.time;
                        state.pos = s_position;
			state.velocity = s_velocity;
                        m_BufferedState[0] = state;
			
                        // Increment state count but never exceed buffer size
                        m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

                        // Check integrity, lowest numbered state in the buffer is newest and so on
                        for (int i = 0; i < m_TimestampCount-1; i++)
                        {
	                        if (m_BufferedState[i].timestamp < m_BufferedState[i+1].timestamp) {
	                                Debug.Log("State inconsistent");
				}
                        }	
		}
	}	
}
