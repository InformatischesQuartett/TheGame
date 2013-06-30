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
            /* Vertex shader
             * Calculates vertex position and normal in screen space
             * and passes vertex albedo and the UV coordinates
             * to the fragment shader.
             */

            // uniforms
            uniform mat4 FUSEE_M; // model matrix
            uniform mat4 FUSEE_MVP; // model-view-projection matrix
            uniform mat4 FUSEE_ITMV; // inverse transpose model view
            uniform mat4 FUSEE_IT; // inverse transpose matrix

            // attributes
            attribute vec4 fuColor; // vertex albedo
            attribute vec3 fuVertex; // vertex coordinates
            attribute vec3 fuNormal; // vertex normal
            attribute vec2 fuUV; // vertex UV coordinates

            // varyings
            varying vec4 worldVertexPos; // vertex position in world space
            varying vec3 worldVertexNormal; // vertex normal in world space
            varying vec4 vertexPos; // vertex position in screen space
            varying vec3 vertexNormal; // vertex normal in screen space
            varying vec3 vertexColor; // vertex albedo
            varying vec2 vertexUV; // vertex UV coordinates

            // main entry point
            void main()
            {
	            // calculate vertex position and normal in world space
	            worldVertexPos = FUSEE_M * vec4(fuVertex, 1.0);
	            worldVertexNormal = normalize(mat3(FUSEE_IT) * fuNormal);

	            // calculate vertex position and normal in screen space
	            vertexPos = FUSEE_MVP * vec4(fuVertex, 1.0);
	            vertexNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
	
	            // Pass color and UV to fragment shader
	            vertexColor = fuColor;
	            vertexUV = fuUV;
            }";

        /// <summary>
        ///     The fragment shader
        /// </summary>
        private const string FragmentShader = @"
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
	            bool active = false; // switch to activate this light
            };

            struct material
            {
	            vec3 ambient; // material ambient color
	            vec3 diffuse; // material diffuse color
	            vec3 specular; // material specular color
	            float shininess; // specular shininess
            };
 
            // uniforms
            uniform sampler2D tex; // model texture
            uniform int calcLighting = 0; // specifies if the lighting calculation
							              // should be executed or not
            uniform vec3 ambient; // ambient light
            uniform material surfaceMat; // material characteristics
					
            // varyings
            varying vec4 worldVertexPos; // vertex position in world space
            varying vec3 worldVertexNormal; // vertex normal in world space
            varying vec4 vertexPos; // vertex position in screen space
            varying vec3 vertexNormal; // vertex normal in screen space
            varying vec3 vertexColor; // vertex albedo
            varying vec2 vertexUV; // vertex UV coordinates

            // constants
            const int maxLights = 8;

            spotlight lights[maxLights];


            // main entry point
            void main()
            {
	            // initialize lighting with ambient light
	            vec3 totalLighting = ambient * surfaceMat.ambient;
	
	            // iterate over all lights
	            if(calcLighting == 0)
	            {
		            for (int i = 0; i < maxLights; i++)
		            {
			            if(lights[i].active)
			            {
				            totalLighting += calcDiffuse(lights[i]);
			            }
		            }
	            }
	
	            // calculate fragment color
	            gl_FragColor = texture2D(tex, vUV) * totalLighting;
            }

            // diffuse light calculation for a single light
            vec3 calcDiffuse(in spotlight light)
            {
	            float variance = max(0.0, dot(light.direction, vertexNormal));
		
	            float dist = length(light.position - worldVertexPos);
	            float falloff = max(0.0, dist / light.falloff);
		
	            surfaceMat.diffuse * light.diffuse * variance * falloff;
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