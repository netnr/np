/**
 * 2020-05-10
 * 示例拓展
 */

dk_demo(function (dk) {

    var blg = dk.build.language;

    //为 csharp 拓展一个类，指向的是一个有名称的方法，非匿名函数，因为考虑到做调试时，匿名函数会出现格式问题
    // pa 参数是 dk.build.pa() 方法返回的对象，所有的拓展方法都用该参数对象，自己可以在 pa 对象追加参数
    // 所有构建方法输出对象都在 pa.output
    blg.csharp["extendClass"] = function am(pa) {
        //项目命名，同时也是类所属文件夹名
        pa.project = "Netnr.Fast";
        //类的命名空间
        pa.namespace = "Netnr.Fast";

        //输出对象
        pa.output = {
            //对象拓展类
            "Extend.cs": [
                'using System;',
                'using System.Data;',
                'using System.Linq;',
                'using System.Collections.Generic;',
                '',
                'namespace Netnr',
                '{',
                '\t' + '/// <summary>',
                '\t' + '/// 方法拓展',
                '\t' + '/// </summary>',
                '\t' + 'public static class Extend',
                '\t' + '{',
                '\t\t' + '/// <summary>',
                '\t\t' + '/// 实体转表',
                '\t\t' + '/// </summary>',
                '\t\t' + '/// <typeparam name="T">泛型</typeparam>',
                '\t\t' + '/// <param name="list">对象</param>',
                '\t\t' + '/// <returns>返回表</returns>',
                '\t\t' + 'public static DataTable ToDataTable<T>(this IList<T> list)',
                '\t\t' + '{',
                '\t\t\t' + 'Type elementType = typeof(T);',
                '\t\t\t' + 'var t = new DataTable();',
                '\t\t\t' + 'elementType.GetProperties().ToList().ForEach(propInfo => t.Columns.Add(propInfo.Name, Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType));',
                '\t\t\t' + 'foreach (T item in list)',
                '\t\t\t' + '{',
                '\t\t\t\t' + 'var row = t.NewRow();',
                '\t\t\t\t' + 'elementType.GetProperties().ToList().ForEach(propInfo => row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value);',
                '\t\t\t\t' + 't.Rows.Add(row);',
                '\t\t\t' + '}',
                '\t\t' + '}',
                '\t' + '}',
                '}',
            ],
            //发送邮件的类
            "SendEmail.cs": [
            ],
            //发送短信的类
            "SendMsg.cs": [
            ]
        };
    }
    //添加注释
    blg.csharp["extendClass-comment"] = "拓展类";

    //删除 java 的注释
    delete blg["java-comment"];

    //添加新的语言，注意：语种需 Monaco editor 编辑器支持
    blg["javascript"] = {

        basic: function am(pa) {
            pa.output = {
                "fun.js": [
                    '// 日期转数值',
                    'console.log(+new Date());',
                    '',
                    '// 获取ip',
                    'fetch("https://api.zme.ink/ip").then(x => x.json()).then(console.log)'
                ]
            }
        }
    };

    //查看拓展后的构建信息
    console.log(dk.build);


    //重新渲染构建
    dk.viewBuild();

});