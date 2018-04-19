using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tools {
    /// <summary>
    /// Contains static methods used for serialization.
    /// </summary>
    public static class Serial {
        /// <summary>
        /// Serialize the object specified and store it in the specified file.
        /// </summary>
        /// <param name="file">file to store the serialized data.</param>
        /// <param name="obj">Object to serialize.</param>
        public static void Serialize(string file, object obj) {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);

            formatter.Serialize(stream, obj);
            stream.Close();
        }

        /// <summary>
        /// Deserialize the file specified.
        /// </summary>
        /// <param name="file">The serialized file.</param>
        /// <returns>Return the deserialized object.</returns>
        public static object Deserialize(string file) {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            object obj = formatter.Deserialize(stream);
            stream.Close();

            return obj;
        }
    }
}
