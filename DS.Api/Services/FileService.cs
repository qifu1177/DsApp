using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Ds.Application.Models;
using Ds.Infrastructure.Interfaces.Models;
using DS.Api.Base;
using DS.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DS.Api.Services
{
    public class FileService : IFileService
    {
        public FileService() { }
        public virtual string GetFilePath()
        {
            //var action = CreateAction();
            var excutFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string filePath = (new FileInfo(excutFile)).DirectoryName;
            var path = Path.Combine(filePath, "files");

            if (Directory.Exists(path))
                return path;
            var info = new DirectoryInfo(path);
            info.Create();
            return path;
        }
        public virtual string CopyFile(string path, IFormFile file)
        {
            string filePath = Path.Combine(path, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return filePath;
        }
        public virtual string[] GetFiles(string path, string type)
        {
            List<string> files = new List<string>();
            var info = new DirectoryInfo(path);
            foreach (var file in info.GetFiles())
            {
                if (file.Name.EndsWith(type))
                    files.Add(file.Name);
            }
            return files.ToArray();
        }
        public virtual IEnumerable<IDtValue> GetValues(string path, string fileName)
        {
            var csvGetValues = CsvInterpretationFactory.Instance.Create(path, fileName);
            if (csvGetValues != null)
                return csvGetValues.Invoke(path, fileName);
            return new DtValue[0];
        }

        public Action<List<DtValue>, string[]> CreateAction()
        {
            Expression<Func<string[], string>> expression = CreateDateTimeFormatStr();
            Expression<Action<List<DtValue>, string[]>> actionExpression = CreateExpression(expression.Compile());
            return actionExpression.Compile();
        }
        
        public Expression<Func<string[], string>> CreateDateTimeFormatStr()
        {
            Expression<Func<string[], string>>  expression= (strs) => string.Format("{0}-{1}-{2} {4}", strs[2], strs[1], strs[0], strs[1]);
            return expression;
        }
        public Expression<Action<List<DtValue>, string[]>> CreateExpression(Func<string[], string> toDateTimeStr)
        {
            Expression<Action<List<DtValue>,string[]>> expression = (list,strs) => list.Add(
                DtValue.Create(new DateTimeOffset(Convert.ToDateTime(toDateTimeStr(strs))),
                    Convert.ToDouble(strs[5].Replace(',', '.'))));
            return expression;
        }
    }
}
