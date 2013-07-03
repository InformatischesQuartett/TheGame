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
            uniform mat4 FUSEE_M; // model matrix
            uniform mat4 FUSEE_MVP; // model-view-projection matrix
            uniform mat4 FUSEE_ITMV; // inverse transpose model view matrix

            // attributes
            attribute vec4 fuColor; // vertex albedo
            attribute vec3 fuVertex; // vertex coordinates
            attribute vec3 fuNormal; // vertex normal
            attribute vec2 fuUV; // vertex UV coordinates

            // varyings
            varying vec4 worldVertexPos; // vertex position in world space
            varying vec3 worldVertexNormal; // vertex normal in world space
            varying vec3 vertexNormal; // vertex normal in screen space
            varying vec4 vertexColor; // vertex albedo
            varying vec2 vertexUV; // vertex UV coordinates

            // main entry point
            void main()
            {
	            // calculate vertex position and normal in world space
	            worldVertexPos = vec4(fuVertex, 1.0);
	            worldVertexNormal = fuNormal;

	            // calculate vertex position and normal in screen space
	            gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vertexNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
	
	            // Pass color and UV to fragment shader
	            vertexColor = fuColor;
	            vertexUV = fuUV;
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

            // structs
            struct spotlight
            {
	            vec4 position; // light position in world space
	            vec3 direction; // spot direction
	            vec4 diffuse; // diffuse color of the light
	            vec4 specular; // specular color of the light
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

            uniform int amountOfLights = 8; // Amount of lights to calculate

            uniform vec4 light1Position; // light position in world space
            uniform vec3 light1Direction; // spot direction
            uniform vec4 light1Diffuse; // diffuse color of the light
            uniform vec4 light1Specular; // specular color of the light
            uniform float light1Falloff; // light attenuation
            uniform vec4 light2Position; // light position in world space
            uniform vec3 light2Direction; // spot direction
            uniform vec4 light2Diffuse; // diffuse color of the light
            uniform vec4 light2Specular; // specular color of the light
            uniform float light2Falloff; // light attenuation
            uniform vec4 light3Position; // light position in world space
            uniform vec3 light3Direction; // spot direction
            uniform vec4 light3Diffuse; // diffuse color of the light
            uniform vec4 light3Specular; // specular color of the light
            uniform float light3Falloff; // light attenuation
            uniform vec4 light4Position; // light position in world space
            uniform vec3 light4Direction; // spot direction
            uniform vec4 light4Diffuse; // diffuse color of the light
            uniform vec4 light4Specular; // specular color of the light
            uniform float light4Falloff; // light attenuation
            uniform vec4 light5Position; // light position in world space
            uniform vec3 light5Direction; // spot direction
            uniform vec4 light5Diffuse; // diffuse color of the light
            uniform vec4 light5Specular; // specular color of the light
            uniform float light5Falloff; // light attenuation
            uniform vec4 light6Position; // light position in world space
            uniform vec3 light6Direction; // spot direction
            uniform vec4 light6Diffuse; // diffuse color of the light
            uniform vec4 light6Specular; // specular color of the light
            uniform float light6Falloff; // light attenuation
            uniform vec4 light7Position; // light position in world space
            uniform vec3 light7Direction; // spot direction
            uniform vec4 light7Diffuse; // diffuse color of the light
            uniform vec4 light7Specular; // specular color of the light
            uniform float light7Falloff; // light attenuation
            uniform vec4 light8Position; // light position in world space
            uniform vec3 light8Direction; // spot direction
            uniform vec4 light8Diffuse; // diffuse color of the light
            uniform vec4 light8Specular; // specular color of the light
            uniform float light8Falloff; // light attenuation
					
            // varyings
            varying vec4 worldVertexPos; // vertex position in world space
            varying vec3 worldVertexNormal; // vertex normal in world space
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
				            lights[i] = spotlight(light1Position, light1Direction, light1Diffuse, light1Specular, light1Falloff);
			            }
			            if(i == 1)
			            {
				            lights[i] = spotlight(light2Position, light2Direction, light2Diffuse, light2Specular, light2Falloff);
			            }
			            if(i == 2)
			            {
				            lights[i] = spotlight(light3Position, light3Direction, light3Diffuse, light3Specular, light3Falloff);
			            }
			            if(i == 3)
			            {
				            lights[i] = spotlight(light4Position, light4Direction, light4Diffuse, light4Specular, light4Falloff);
			            }
			            if(i == 4)
			            {
				            lights[i] = spotlight(light5Position, light5Direction, light5Diffuse, light5Specular, light5Falloff);
			            }
			            if(i == 5)
			            {
				            lights[i] = spotlight(light6Position, light6Direction, light6Diffuse, light6Specular, light6Falloff);
			            }
			            if(i == 6)
			            {
				            lights[i] = spotlight(light7Position, light7Direction, light7Diffuse, light7Specular, light7Falloff);
			            }
			            if(i == 7)
			            {
				            lights[i] = spotlight(light8Position, light8Direction, light8Diffuse, light8Specular, light8Falloff);
			            }
		            }
	            }
            }

            // diffuse light calculation for a single light
            vec4 calcDiffuse(in spotlight light)
            {
	            float variance = max(0.0, dot(light.direction, worldVertexNormal));
		
	            float dist = length(light.position - worldVertexPos);
	            float falloff = max(0.0, (-dist / light.falloff) + 1);
		
	            return matDiffuse * light.diffuse * variance * falloff;
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
				            totalLighting += calcDiffuse(lights[i]);
			            }
                        else
                        {
                            break;
                        }
		            }
	            }
	
	            // calculate fragment color
	            gl_FragColor = texture2D(tex, vertexUV) * totalLighting;
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