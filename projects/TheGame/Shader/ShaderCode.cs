namespace Examples.TheGame.Shader
{
    /// <summary>
    ///     Contains the code of vertex and frament shader
    /// </summary>
    internal class ShaderCode
    {
        /// <summary>
        ///     The vertex shader
        /// </summary>
        private const string VertexShader = @"
            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
            
        
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
                // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
                // vNormal = norm4.xyz;
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
                vUV = fuUV;
            }";

        /// <summary>
        ///     The fragment shader
        /// </summary>
        private const string FragmentShader = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

            //The parameter required for the texturing process
            uniform sampler2D texture1;
            uniform vec3 lightDir; // Directional light
            uniform vec4 lightColor;
            uniform vec4 ambientLight;
            uniform int receiveShadows; // FUSEE seems not to support bools?!
            uniform vec4 vColor;
            varying vec3 vNormal;
            //The parameter holding the UV-Coordinates of the texture
            varying vec2 vUV;

            void main()
            {    
              // Diffuse
              float variance = dot(lightDir, vNormal);

              gl_FragColor = texture2D(texture1, vUV);   
              if(variance < 0.0)
                 variance = 0.0;
              if(receiveShadows != 0)
			     gl_FragColor *= (variance * lightColor) + ambientLight;
            }";

        /// <summary>
        ///     Gets the vertex shader raw code
        /// </summary>
        /// <returns>The uncompiled code of the vertex shader</returns>
        public static string GetVertexShader()
        {
            return VertexShader;
        }

        /// <summary>
        ///     Gets the pixel shader raw code
        /// </summary>
        /// <returns>The uncompiled code of the fragment shader</returns>
        public static string GetFragmentShader()
        {
            return FragmentShader;
        }
    }
}