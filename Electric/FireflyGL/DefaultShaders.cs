using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	class DefaultShaders
	{
		public static string ShapeFrag { get; set; }
		public static string ShapeVert { get; set; }
		public static string TexturedFrag { get; set; }
		public static string TexturedVert { get; set; }
		public static string OGL4ShapeFrag { get; set; }
		public static string OGL4ShapeVert { get; set; }
		public static string OGL4TexturedFrag { get; set; }
		public static string OGL4TexturedVert { get; set; }


		static DefaultShaders()
		{
			if (File.Exists("shaders/defShapeFrag.frag.c"))
			{
				ShapeFrag = Utility.Utility.LoadTextFromFile("shaders/defShapeFrag.frag.c");
			}
			else
			{
				ShapeFrag = @"
#version 110

uniform float alpha;
uniform float tintR;
uniform float tintG;
uniform float tintB;
uniform float tintA;

varying vec4 frag_color;

void main () 
{
	vec3 tint = vec3(tintR, tintG, tintB);
	gl_FragColor = vec4(frag_color.rgb * (1.0 - tintA) + tint * tintA, frag_color.a * alpha);
}";
			}

			if (File.Exists("shaders/defShapeVert.vert.c"))
			{
				ShapeVert = Utility.Utility.LoadTextFromFile("shaders/defShapeVert.vert.c");
			}
			else
			{
				ShapeVert = @"
#version 110

attribute vec4 vertex_coord;
attribute vec4 vertex_color;

uniform mat4 window_matrix;
uniform mat4 model_matrix;
uniform mat4 projection_matrix;
uniform mat4 camera_matrix;

varying vec4 frag_color;

void main () 
{
	frag_color = vertex_color;
	gl_Position = projection_matrix * camera_matrix * model_matrix * vertex_coord;
} ";
			}

			if (File.Exists("shaders/defTextureFrag.frag.c"))
			{
				TexturedFrag = Utility.Utility.LoadTextFromFile("shaders/defTextureFrag.frag.c");
			}
			else
			{
				TexturedFrag = @"
#version 110

uniform float alpha;
uniform float tintR;
uniform float tintG;
uniform float tintB;
uniform float tintA;
uniform sampler2D texture;

varying vec2 frag_texcoord;

void main () 
{
	vec3 tint = vec3(tintR, tintG, tintB);
	vec4 tempColor = texture2D(texture, frag_texcoord - floor( frag_texcoord ) );
	gl_FragColor = vec4(tempColor.rgb * (1.0 - tintA) + tint * tintA, tempColor.a * alpha);
}";
			}
			if (File.Exists("shaders/defTextureVert.vert.c"))
			{
				TexturedVert = Utility.Utility.LoadTextFromFile("shaders/defTextureVert.vert.c");
			}
			else
			{
				TexturedVert = @"
#version 110

attribute vec4 vertex_coord;
attribute vec2 vertex_texcoord;

uniform mat4 window_matrix;
uniform mat4 model_matrix;
uniform mat4 projection_matrix;
uniform mat4 camera_matrix;

varying vec2 frag_texcoord;

void main () {

	frag_texcoord = vertex_texcoord;
	gl_Position = window_matrix * projection_matrix * camera_matrix * model_matrix * vertex_coord;
}";
			}


			//Load OGL4 shaders
			//======================================================================================

			if (File.Exists("shaders/defOGL4ShapeFrag.frag.c"))
			{
				OGL4ShapeFrag = Utility.Utility.LoadTextFromFile("shaders/defOGL4ShapeFrag.frag.c");
			}
			else
			{
				OGL4ShapeFrag = @"
#version 150

uniform float alpha;
uniform float tintR;
uniform float tintG;
uniform float tintB;
uniform float tintA;

in vec4 frag_color;
out vec4 FragColor;

void main () 
{
	vec3 tint = vec3(tintR, tintG, tintB);
	FragColor = vec4(frag_color.rgb * (1.0 - tintA) + tint * tintA, frag_color.a * alpha);
}";
			}

			if (File.Exists("shaders/defOGL4ShapeVert.vert.c"))
			{
				OGL4ShapeVert = Utility.Utility.LoadTextFromFile("shaders/defOGL4ShapeVert.vert.c");
			}
			else
			{
				OGL4ShapeVert = @"
#version 150

in vec4 vertex_coord;
in vec4 vertex_color;

uniform mat4 window_matrix;
uniform mat4 model_matrix;
uniform mat4 projection_matrix;
uniform mat4 camera_matrix;

out vec4 frag_color;

void main () {

	frag_color = vertex_color;
	gl_Position = projection_matrix * camera_matrix * model_matrix * vertex_coord;
} ";
			}

			if (File.Exists("shaders/defOGL4TextureFrag.frag.c"))
			{
				OGL4TexturedFrag = Utility.Utility.LoadTextFromFile("shaders/defOGL4TextureFrag.frag.c");
			}
			else
			{
				OGL4TexturedFrag = @"
#version 150

uniform float alpha;
uniform float tintR;
uniform float tintG;
uniform float tintB;
uniform float tintA;
uniform sampler2D texture;

in vec2 frag_texcoord;
out vec4 FragColor;

void main () 
{
	vec3 tint = vec3(tintR, tintG, tintB);
	vec4 tempColor = texture2D(texture, frag_texcoord - floor( frag_texcoord ) );
	FragColor = vec4(tempColor.rgb * (1.0 - tintA) + tint * tintA, tempColor.a * alpha);
}";
			}
			if (File.Exists("shaders/defOGL4TextureVert.vert.c"))
			{
				OGL4TexturedVert = Utility.Utility.LoadTextFromFile("shaders/defOGL4TextureVert.vert.c");
			}
			else
			{
				OGL4TexturedVert = @"
#version 150

in vec4 vertex_coord;
in vec2 vertex_texcoord;

uniform mat4 window_matrix;
uniform mat4 model_matrix;
uniform mat4 projection_matrix;
uniform mat4 camera_matrix;

out vec2 frag_texcoord;

void main () {

	frag_texcoord = vertex_texcoord;
	gl_Position = window_matrix * projection_matrix * camera_matrix * model_matrix * vertex_coord;
}";
			}
		}
	}
}
