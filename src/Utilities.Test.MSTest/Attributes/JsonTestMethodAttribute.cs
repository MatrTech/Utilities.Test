using System;
using System.Collections;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace Matr.Utilities.Test.Attributes
{
    [AttributeUsage(System.AttributeTargets.Method)]
    public partial class JsonTestMethodAttribute : TestMethodAttribute
    {
        private readonly string filepath;
        private readonly Type dataType;
        public JsonTestMethodAttribute(string filepath, Type dataType)
        {
            if (string.IsNullOrWhiteSpace(filepath))
                throw new ArgumentNullException(nameof(filepath));

            _ = File.Exists(filepath) ? filepath : throw new FileNotFoundException(filepath);
            _ = dataType ?? throw new ArgumentNullException(nameof(dataType));

            this.filepath = filepath;
            this.dataType = dataType;
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            using (StreamReader r = new StreamReader(filepath))
            {
                string json = r.ReadToEnd();

                var dataArray = JsonSerializer.Deserialize(json, dataType.MakeArrayType());
#if NETSTANDARD2_0
                return (dataArray as IEnumerable).Cast<object>()
#else
                return (dataArray as IEnumerable)!.Cast<object>()
#endif
                    .Select(x => testMethod.Invoke(new object[] { x }))
                    .ToArray();
            }
        }
    }
}