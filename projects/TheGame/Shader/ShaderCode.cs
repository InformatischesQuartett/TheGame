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
            #version 120

            /* Vertex shader
             * Calculates vertex position and normal in screen space
             * and passes vertex albedo and the UV coordinates
             * to the fragment shader.
             */

            // uniforms
            uniform mat4 FUSEE_MV; // model-view-projection matrix
            uniform mat4 FUSEE_MVP; // model-view-projection matrix
            uniform mat4 FUSEE_ITMV; // inverse transposed model view matrix

            // attributes
            attribute vec4 fuColor; // vertex albedo
            attribute vec3 fuVertex; // vertex coordinates
            attribute vec3 fuNormal; // vertex normal
            attribute vec2 fuUV; // vertex UV coordinates

            // varyings
            varying vec4 vertexPos; // vertex position in screen space
            varying vec3 vertexNormal; // vertex normal in screen space
            varying vec4 vertexColor; // vertex albedo
            varying vec2 vertexUV; // vertex UV coordinates

            // main entry point
            void main()
            {                 
	            // Pass color and UV to fragment shader
	            vertexColor = fuColor;
	            vertexUV = fuUV;

                // calculate vertex position and normal in screen space
                vertexNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
                vertexPos = FUSEE_MV * vec4(fuVertex, 1.0);
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
            }";

        /// <summary>
        ///     The fragment shader
        /// </summary>
        private const string FragmentShader = @"
            #version 120

            /* Fragment shader
             * Calculates the diffuse component for up to 8 spot lights.
             * Supports material characteristics and can also switch off
             * the light calculation completely.
             */
 
             #define M_PI 3.1415926535897932384626433832795

            // structs
            struct spotlight
            {
	            vec4 position; // light position in eye space
	            vec3 direction; // spot direction
	            vec4 diffuse; // diffuse color of the light
	            vec4 specular; // specular color of the light
	            float aperture; // aperture of the cone of light (i.e. the angle 
					            // between the vector from apex to the middle of 
					            // basement and the cone's surface) in radians
	            float falloff; // light attenuation
            };
 
            // uniforms
            uniform sampler2D tex; // model texture
            uniform int calcLighting = 0; // specifies if the lighting calculation
							              // should be executed or not
            uniform vec4 ambientLight; // ambient light
            uniform vec4 matAmbient; // material ambient color
            uniform vec4 matDiffuse; // material diffuse color
            uniform vec4 matSpecular; // material specular color
            uniform float matShininess; // specular shininess
            uniform vec4 matAlbedo = vec4(0.5, 0.5, 1, 1); // Albedo of the material

            uniform vec4 camPosition; // camera position in eye space

            uniform int amountOfLights = 8; // Amount of lights to calculate

            uniform vec4 light1Position; // light position in world space
            uniform vec3 light1Direction; // spot direction
            uniform vec4 light1Diffuse; // diffuse color of the light
            uniform vec4 light1Specular; // specular color of the light
            uniform float light1Aperture = M_PI / 6; // aperture of the cone of light
            uniform float light1Falloff; // light attenuation
            uniform vec4 light2Position; // light position in world space
            uniform vec3 light2Direction; // spot direction
            uniform vec4 light2Diffuse; // diffuse color of the light
            uniform vec4 light2Specular; // specular color of the light
            uniform float light2Aperture = M_PI / 6; // aperture of the cone of light
            uniform float light2Falloff; // light attenuation
            uniform vec4 light3Position; // light position in world space
            uniform vec3 light3Direction; // spot direction
            uniform vec4 light3Diffuse; // diffuse color of the light
            uniform vec4 light3Specular; // specular color of the light
            uniform float light3Aperture = M_PI / 6; // aperture of the cone of light
            uniform float light3Falloff; // light attenuation
            uniform vec4 light4Position; // light position in world space
            uniform vec3 light4Direction; // spot direction
            uniform vec4 light4Diffuse; // diffuse color of the light
            uniform vec4 light4Specular; // specular color of the light
            uniform float light4Aperture = M_PI / 6; // aperture of the cone of light
            uniform float light4Falloff; // light attenuation
            uniform vec4 light5Position; // light position in world space
            uniform vec3 light5Direction; // spot direction
            uniform vec4 light5Diffuse; // diffuse color of the light
            uniform vec4 light5Specular; // specular color of the light
            uniform float light5Aperture = M_PI / 6; // aperture of the cone of light
            uniform float light5Falloff; // light attenuation
            uniform vec4 light6Position; // light position in world space
            uniform vec3 light6Direction; // spot direction
            uniform vec4 light6Diffuse; // diffuse color of the light
            uniform vec4 light6Specular; // specular color of the light
            uniform float light6Aperture = M_PI / 6; // aperture of the cone of light
            uniform float light6Falloff; // light attenuation
            uniform vec4 light7Position; // light position in world space
            uniform vec3 light7Direction; // spot direction
            uniform vec4 light7Diffuse; // diffuse color of the light
            uniform vec4 light7Specular; // specular color of the light
            uniform float light7Aperture = M_PI / 6; // aperture of the cone of light
            uniform float light7Falloff; // light attenuation
            uniform vec4 light8Position; // light position in world space
            uniform vec3 light8Direction; // spot direction
            uniform vec4 light8Diffuse; // diffuse color of the light
            uniform vec4 light8Specular; // specular color of the light
            uniform float light8Aperture = M_PI / 6; // aperture of the cone of light
            uniform float light8Falloff; // light attenuation

            uniform float noiseStrength = 0;
            uniform float noiseTime = 0;
            uniform vec2 noiseOffset = vec2(0, 0);
					
            // varyings
            varying vec4 vertexPos; // vertex position in screen space
            varying vec3 vertexNormal; // vertex normal in screen space
            varying vec4 vertexColor; // vertex albedo
            varying vec2 vertexUV; // vertex UV coordinates

            // constants
            const int maxLights = 8;

            spotlight lights[maxLights];


            // initializes the light array with all necessary lights
            void initLights()
            {
	            for (int i = 0; i < maxLights; i++)
	            {
		            if(i < amountOfLights)
		            {
			            // This is awful :(
			            if(i == 0)
			            {
				            lights[i] = spotlight(light1Position, normalize(light1Direction), light1Diffuse, light1Specular, light1Aperture, light1Falloff);
			            }
			            if(i == 1)
			            {
				            lights[i] = spotlight(light2Position, normalize(light2Direction), light2Diffuse, light2Specular, light2Aperture, light2Falloff);
			            }
			            if(i == 2)
			            {
				            lights[i] = spotlight(light3Position, normalize(light3Direction), light3Diffuse, light3Specular, light3Aperture, light3Falloff);
			            }
			            if(i == 3)
			            {
				            lights[i] = spotlight(light4Position, normalize(light4Direction), light4Diffuse, light4Specular, light4Aperture, light4Falloff);
			            }
			            if(i == 4)
			            {
				            lights[i] = spotlight(light5Position, normalize(light5Direction), light5Diffuse, light5Specular, light5Aperture, light5Falloff);
			            }
			            if(i == 5)
			            {
				            lights[i] = spotlight(light6Position, normalize(light6Direction), light6Diffuse, light6Specular, light6Aperture, light6Falloff);
			            }
			            if(i == 6)
			            {
				            lights[i] = spotlight(light7Position, normalize(light7Direction), light7Diffuse, light7Specular, light7Aperture, light7Falloff);
			            }
			            if(i == 7)
			            {
				            lights[i] = spotlight(light8Position, normalize(light8Direction), light8Diffuse, light8Specular, light8Aperture, light8Falloff);
			            }
		            }
	            }
            }

            // Checks if a point is inside the cone of light
            bool isInConeOfLight(in vec3 point, in spotlight light)
            {
                return true;
	            vec3 apexToPoint = normalize(point - vec3(light.position));
	            return dot(apexToPoint, light.direction) / length(apexToPoint) / length(light.direction) > cos(light.aperture);
            }

            // diffuse light calculation for a single light
            vec4 calcDiffuse(in spotlight light, float falloff)
            {
	            float variance = max(0.0, dot(-light.direction, normalize(vertexNormal)));
		
	            return matDiffuse * light.diffuse * variance * falloff;
            }

            // specular reflection calculation
            vec4 calcSpecular(in spotlight light, float falloff)
            {
	            vec4 specular;
	            if(dot(vertexNormal, light.direction) < 0.0)
	            {
		            // light source is on the wrong side so there is no specular component
		            specular = vec4(0.0, 0.0, 0.0, 0.0);
	            }
	            else
	            {
		            vec3 viewDirection = vec3(normalize(vertexPos - camPosition));
		            vec3 normalDirection = normalize(vertexNormal);
		            specular = falloff * light.specular * matSpecular * pow(max(0.0, dot(reflect(-light.direction, normalDirection), viewDirection)), matShininess);
	            }
	            return specular;
            }

            // classic perlin noise implementation
            // by Stefan Gustavson (stefan.gustavson@liu.se),
            // distributed under the MIT license.
            // used to generate some noise on top of the texture
            vec3 mod289(vec3 x)
            {
              return x - floor(x * (1.0 / 289.0)) * 289.0;
            }
            vec4 mod289(vec4 x)
            {
              return x - floor(x * (1.0 / 289.0)) * 289.0;
            }
            vec4 permute(vec4 x)
            {
              return mod289(((x*34.0)+1.0)*x);
            }
            vec4 taylorInvSqrt(vec4 r)
            {
              return 1.79284291400159 - 0.85373472095314 * r;
            }
            vec3 fade(vec3 t) {
              return t*t*t*(t*(t*6.0-15.0)+10.0);
            }
            // Classic Perlin noise
            float cnoise(vec3 P)
            {
              vec3 Pi0 = floor(P); // Integer part for indexing
              vec3 Pi1 = Pi0 + vec3(1.0); // Integer part + 1
              Pi0 = mod289(Pi0);
              Pi1 = mod289(Pi1);
              vec3 Pf0 = fract(P); // Fractional part for interpolation
              vec3 Pf1 = Pf0 - vec3(1.0); // Fractional part - 1.0
              vec4 ix = vec4(Pi0.x, Pi1.x, Pi0.x, Pi1.x);
              vec4 iy = vec4(Pi0.yy, Pi1.yy);
              vec4 iz0 = Pi0.zzzz;
              vec4 iz1 = Pi1.zzzz;

              vec4 ixy = permute(permute(ix) + iy);
              vec4 ixy0 = permute(ixy + iz0);
              vec4 ixy1 = permute(ixy + iz1);

              vec4 gx0 = ixy0 * (1.0 / 7.0);
              vec4 gy0 = fract(floor(gx0) * (1.0 / 7.0)) - 0.5;
              gx0 = fract(gx0);
              vec4 gz0 = vec4(0.5) - abs(gx0) - abs(gy0);
              vec4 sz0 = step(gz0, vec4(0.0));
              gx0 -= sz0 * (step(0.0, gx0) - 0.5);
              gy0 -= sz0 * (step(0.0, gy0) - 0.5);

              vec4 gx1 = ixy1 * (1.0 / 7.0);
              vec4 gy1 = fract(floor(gx1) * (1.0 / 7.0)) - 0.5;
              gx1 = fract(gx1);
              vec4 gz1 = vec4(0.5) - abs(gx1) - abs(gy1);
              vec4 sz1 = step(gz1, vec4(0.0));
              gx1 -= sz1 * (step(0.0, gx1) - 0.5);
              gy1 -= sz1 * (step(0.0, gy1) - 0.5);

              vec3 g000 = vec3(gx0.x,gy0.x,gz0.x);
              vec3 g100 = vec3(gx0.y,gy0.y,gz0.y);
              vec3 g010 = vec3(gx0.z,gy0.z,gz0.z);
              vec3 g110 = vec3(gx0.w,gy0.w,gz0.w);
              vec3 g001 = vec3(gx1.x,gy1.x,gz1.x);
              vec3 g101 = vec3(gx1.y,gy1.y,gz1.y);
              vec3 g011 = vec3(gx1.z,gy1.z,gz1.z);
              vec3 g111 = vec3(gx1.w,gy1.w,gz1.w);

              vec4 norm0 = taylorInvSqrt(vec4(dot(g000, g000), dot(g010, g010), dot(g100, g100), dot(g110, g110)));
              g000 *= norm0.x;
              g010 *= norm0.y;
              g100 *= norm0.z;
              g110 *= norm0.w;
              vec4 norm1 = taylorInvSqrt(vec4(dot(g001, g001), dot(g011, g011), dot(g101, g101), dot(g111, g111)));
              g001 *= norm1.x;
              g011 *= norm1.y;
              g101 *= norm1.z;
              g111 *= norm1.w;

              float n000 = dot(g000, Pf0);
              float n100 = dot(g100, vec3(Pf1.x, Pf0.yz));
              float n010 = dot(g010, vec3(Pf0.x, Pf1.y, Pf0.z));
              float n110 = dot(g110, vec3(Pf1.xy, Pf0.z));
              float n001 = dot(g001, vec3(Pf0.xy, Pf1.z));
              float n101 = dot(g101, vec3(Pf1.x, Pf0.y, Pf1.z));
              float n011 = dot(g011, vec3(Pf0.x, Pf1.yz));
              float n111 = dot(g111, Pf1);

              vec3 fade_xyz = fade(Pf0);
              vec4 n_z = mix(vec4(n000, n100, n010, n110), vec4(n001, n101, n011, n111), fade_xyz.z);
              vec2 n_yz = mix(n_z.xy, n_z.zw, fade_xyz.y);
              float n_xyz = mix(n_yz.x, n_yz.y, fade_xyz.x); 
              return n_xyz;
            }
            float surface3 (vec3 coord) {
	
	            float frequency = 4.0;
	            float n = 0.0;	
		
	            n += 0.33	* ( 0.5+2.0*cnoise( coord * frequency ) );
	            n += 0.33	* ( 0.5+2.0*cnoise( coord * frequency * 2.0 ) );
	            n += 0.33	* ( 0.5+2.0*cnoise( coord * frequency * 4.0 ) );
	
	            return n;
            }

            // calculates the noise pattern on top of the texture
            vec4 calcNoise()
            {
	            vec2 position = vertexUV.st + noiseOffset;
	            float n = (1 - noiseStrength) + (surface3(vec3(position, noiseTime)) * noiseStrength);
	            return vec4(n, n, n, 1.0);
            }

            // main entry point
            void main()
            {
	            // initialize lighting with ambient light
	            vec4 totalLighting = ambientLight * matAmbient;
	
	            // iterate over all lights
	            if(calcLighting == 0)
	            {
		            // add all available lights into the array
		            initLights();

		            // Calculate lighting
		            for (int i = 0; i < maxLights; i++)
		            {
			            if(i < amountOfLights)
			            {
				            if(isInConeOfLight(vec3(vertexPos), lights[i]))
				            {
					            // calculate falloff
					            float dist = length(vec3(vertexPos) - vec3(lights[i].position));
					            float falloff = max(0.0, (-dist / lights[i].falloff) + 1);
                                if(lights[i].falloff == 0)
                                    falloff = 1;
	
					            totalLighting += calcDiffuse(lights[i], falloff);
					            totalLighting += calcSpecular(lights[i], falloff);
				            }
			            }
			            else
			            {
				            break;
			            }
		            }
	            }
	
	            // calculate some noise
	            if(noiseStrength > 0)
	            {
		            totalLighting *= calcNoise();
	            }
	
	            // calculate fragment color
                vec4 texColor = texture2D(tex, vertexUV);
                if(texColor.rgb == vec3(0, 0, 0))
                    texColor.rgb = vec3(0.5, 0.5, 1.0);
	            //gl_FragColor = ((1 -texColor.a) * matAlbedo + texColor) * totalLighting;
                gl_FragColor = texColor * totalLighting;
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