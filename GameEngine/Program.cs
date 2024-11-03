using GLFW;
using static OpenGL.GL;
using static OpenGL.GLFW;
using System.Numerics;
namespace GameEngine;


public static class MainClass
{
    private const string TITLE = "test!";

    static void Main(string[] args)
    {
        var window = CreateWindow(800, 600);
        PrepareContext();

        uint shaderProgram = SetupShaders();

        float[] vertices = {
                0.5f,  0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                -0.5f, -0.5f, 0.0f,
                -0.5f,  0.5f, 0.0f
            };
        uint[] indices = {
                0, 1, 3,
                1, 2, 3
            };

        uint VAO, VBO, EBO;
        CreateVertices(out VAO, out VBO, out EBO, vertices, indices);



        while (!Glfw.WindowShouldClose(window))
        {
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            glUseProgram(shaderProgram);

            Matrix4x4 view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY);
            Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView((float)ToRadians(45), 800f / 600f, 0.1f, 100f);

            int modelLoc = glGetUniformLocation(shaderProgram, "model");
            int viewLoc = glGetUniformLocation(shaderProgram, "view");
            int projLoc = glGetUniformLocation(shaderProgram, "projection");

            unsafe
            {
                glUniformMatrix4fv(viewLoc, 1, false, (float*)Unsafe.AsPointer(ref view));
                glUniformMatrix4fv(projLoc, 1, false, (float*)Unsafe.AsPointer(ref projection));
            }

            foreach (var entity in activeScene.GameEntities)
            {
                var transform = entity.Transform;
                Matrix4x4 model = Matrix4x4.CreateTranslation(transform.Position) *
                                  Matrix4x4.CreateRotationX(transform.Rotation.X) *
                                  Matrix4x4.CreateRotationY(transform.Rotation.Y) *
                                  Matrix4x4.CreateRotationZ(transform.Rotation.Z) *
                                  Matrix4x4.CreateScale(transform.Scale);

                unsafe
                {
                    glUniformMatrix4fv(modelLoc, 1, false, (float*)Unsafe.AsPointer(ref model));
                }

                glBindVertexArray(entity.VAO);
                glDrawElements(GL_TRIANGLES, entity.Indices.Length, GL_UNSIGNED_INT, IntPtr.Zero);
            }

            Glfw.SwapBuffers(window);
            Glfw.PollEvents();
        }
    }

    public static double ToRadians(this double val)
    {
        return Math.PI / 180 * val;
    }

    private static unsafe void CreateVertices(out uint VAO, out uint VBO, out uint EBO, float[] vertices, uint[] indices)
    {
        VAO = glGenVertexArray();
        VBO = glGenBuffer();
        EBO = glGenBuffer();

        glBindVertexArray(VAO);
        glBindBuffer(GL_ARRAY_BUFFER, VBO);

        fixed (float* verticesPtr = vertices)
        {
            glBufferData(GL_ARRAY_BUFFER, vertices.Length * sizeof(float), (IntPtr)verticesPtr, GL_STATIC_DRAW);
        }

        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);

        fixed (uint* indicesPtr = indices)
        {
            glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.Length * sizeof(uint), (IntPtr)indicesPtr, GL_STATIC_DRAW);
        }

        glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), IntPtr.Zero);
        glEnableVertexAttribArray(0);
        glBindBuffer(GL_ARRAY_BUFFER, 0);
        glBindVertexArray(0);
    }

    private static void PrepareContext()
    {
        Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
        Glfw.WindowHint(Hint.ContextVersionMajor, 3);
        Glfw.WindowHint(Hint.ContextVersionMinor, 3);
        Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
        Glfw.WindowHint(Hint.Doublebuffer, true);
        Glfw.WindowHint(Hint.Decorated, true);
    }

    private static Window CreateWindow(int width, int height)
    {
        var window = Glfw.CreateWindow(width, height, TITLE, GLFW.Monitor.None, Window.None);

        var screen = Glfw.PrimaryMonitor.WorkArea;
        var x = (screen.Width - width) / 2;
        var y = (screen.Height - height) / 2;
        Glfw.SetWindowPosition(window, x, y);

        Glfw.MakeContextCurrent(window);
        Import(Glfw.GetProcAddress);

        return window;
    }

    private static uint SetupShaders()
    {
        const string FragmentShaderSource = "#version 330 core\n" +
                                            "out vec4 FragColor;\n" +
                                            "in vec2 TexCoord;\n" +
                                            "uniform sampler2D texture1;\n" +
                                            "void main()\n" +
                                            "{\n" +
                                            "   FragColor = texture(texture1, TexCoord);\n" +
                                            "}\0";
        const string VertexShaderSource = "#version 330 core\n" +
                                          "layout(location = 0) in vec3 aPos;\n" +
                                          "layout(location = 1) in vec2 aTexCoord;\n" +
                                          "out vec2 TexCoord;\n" +
                                          "uniform mat4 projection;\n" +
                                          "uniform mat4 view;\n" +
                                          "uniform mat4 model;\n" +
                                          "void main()\n" +
                                          "{\n" +
                                          "   gl_Position = projection * view * model * vec4(aPos, 1.0f);\n" +
                                          "   TexCoord = aTexCoord;\n" +
                                          "}\0";

        uint vertexShader = glCreateShader(GL_VERTEX_SHADER);
        glShaderSource(vertexShader, VertexShaderSource);
        glCompileShader(vertexShader);
        //CheckShaderCompileStatus(vertexShader);

        uint fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
        glShaderSource(fragmentShader, FragmentShaderSource);
        glCompileShader(fragmentShader);
        //CheckShaderCompileStatus(fragmentShader);

        uint shaderProgram = glCreateProgram();
        glAttachShader(shaderProgram, vertexShader);
        glAttachShader(shaderProgram, fragmentShader);
        glLinkProgram(shaderProgram);
        //CheckProgramLinkStatus(shaderProgram);

        glDeleteShader(vertexShader);
        glDeleteShader(fragmentShader);

        return shaderProgram;
    }

    //private static void CheckShaderCompileStatus(uint shader)
    //{
    //    int success;
    //    glGetShaderiv(shader, GL_COMPILE_STATUS, &success);
    //    if (success == 0)
    //    {
    //        byte[] infoLog = new byte[512];
    //        glGetShaderInfoLog(shader, 512, null, infoLog);
    //        Console.WriteLine("ERROR::SHADER::COMPILATION_FAILED\n" + System.Text.Encoding.UTF8.GetString(infoLog));
    //    }
    //}

    //private static void CheckProgramLinkStatus(uint program)
    //{
    //    int success;
    //    glGetProgramiv(program, GL_LINK_STATUS, &success);
    //    if (success == 0)
    //    {
    //        byte[] infoLog = new byte[512];
    //        glGetProgramInfoLog(program, 512, null, infoLog);
    //        Console.WriteLine("ERROR::PROGRAM::LINKING_FAILED\n" + System.Text.Encoding.UTF8.GetString(infoLog));
    //    }
    //}
}
