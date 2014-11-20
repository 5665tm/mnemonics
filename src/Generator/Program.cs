using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Ionic.Zip;
using Types;
using DotNet;
using CSharp;

namespace ExlainSoftware
{
	static class Generator
	{
		private static string _version = "0.5";

/*
type StringBuilder with
  member x.AppendString (s:string) = ignore <| x.Append s
  member x.AppendStrings (ss:string list) =
    for s in ss do ignore <| x.Append s


let rec pairs l = seq {
  for a in l do
    for b in l do 
      yield (a,b)
  }

*/

		private static string NewGuid()
		{
			return Guid.NewGuid().ToString().ToLower();
		}

		private static void RenderResharper()
		{
			var te = new TemplatesExport(){family =  "Live Templates"};
			var templates = new List<TemplatesExportTemplate>();
/*
  // debugging switches :)
  let renderCSharp = true

  let printExpressions expressions (vars:List<TemplatesExportTemplateVariable>) defValue =
    let rec impl exps (builder:StringBuilder) =
      match exps with
      | Text(txt) :: t ->
        builder.AppendString txt
        impl t builder

      | DefaultValue :: t ->
        builder.AppendString defValue
        impl t builder

      | DefaultVariable(content) :: t ->
        let v = new TemplatesExportTemplateVariable()
        Console.WriteLine(defValue)
        v.name <- "DefaultVariableSet"
        v.initialRange <- 0
        v.expression <- "constant(\"" + defValue + "\")"
        if not(vars.Any(fun v' -> v.name.Equals(v'.name))) then vars.Add(v)
        builder.AppendStrings ["$"; v.name; "$"]
        impl t builder

      | Variable(name, value) :: t ->
        let v = new TemplatesExportTemplateVariable()
        v.name <- name
        v.initialRange <- 0
        v.expression <- value
        vars.Add(v)
        builder.AppendStrings ["$"; name; "$"]
        impl t builder

      | Constant(name,text) :: t ->
        if name <> "END" then begin
          let v = new TemplatesExportTemplateVariable()
          v.name <- name
          v.initialRange <- 0
          v.expression <- "constant(\"" + text + "\")"
          if not(vars.Any(fun v' -> v.name.Equals(v'.name))) then vars.Add(v)
        end
        builder.AppendStrings ["$"; name; "$"]
        impl t builder
      
      | Scope(content) :: t ->
        builder.AppendString "{"
        impl content builder
        builder.AppendString "}"
        impl t builder

      | FixedType :: t ->
        builder.AppendString "$typename$" // replaced later
        impl t builder
      
      | [] -> ()
    let sb = new StringBuilder()
    impl expressions sb
    sb.ToString();

  // first, process structures
  if renderCSharp then
    for (s,exprs) in cSharpStructureTemplates do
      let t = new TemplatesExportTemplate(shortcut=s)
      let vars = new List<TemplatesExportTemplateVariable>()
      t.description <- String.Empty
      t.reformat <- "True"
      t.uid <- newGuid()
      t.text <- printExpressions exprs vars String.Empty

      t.Context <- new TemplatesExportTemplateContext(CSharpContext = csContext)
      t.Variables <- vars.ToArray()
      templates.Add t
    done

  // now process members
  if renderCSharp then
    for (s,doc,exprs) in cSharpMemberTemplates do
      // simple types; methods can be void
      let types = (if Char.ToLower(s.Chars(0)) ='m' then ("", "void", "") :: csharpTypes else csharpTypes)
      for (tk,tv,defValue) in types do
        let t = new TemplatesExportTemplate(shortcut=(s+tk))
        let vars = new List<TemplatesExportTemplateVariable>()
        t.description <- printExpressions doc vars defValue
        t.reformat <- "True"
        t.shortenQualifiedReferences <- "True"
        t.text <- (printExpressions exprs vars defValue)
                  .Replace("$typename$", if String.IsNullOrEmpty(tv) then "void" else tv)
        t.uid <- newGuid()
        t.Context <- new TemplatesExportTemplateContext(CSharpContext = csContext)
        t.Variables <- vars.ToArray()
        templates.Add t
      done

      // generically specialized types
      for (gk,gv,genArgCount) in dotNetGenericTypes do
        match genArgCount with
        | 1 ->
          for (tk,tv,_) in csharpTypes do
            let t0 = new TemplatesExportTemplate(shortcut=s+gk+tk)
            let vars0 = new List<TemplatesExportTemplateVariable>()
            let genericArgs = gv + "<" + tv + ">"
            let defValue = "new " + genericArgs + "()"
            t0.description <- (printExpressions doc vars0 defValue).Replace("$typename$", genericArgs)
            t0.reformat <- "True"
            t0.shortenQualifiedReferences <- "True"
            t0.text <- (printExpressions exprs vars0 defValue).Replace("$typename$", genericArgs)
            t0.uid <- newGuid()
            t0.Context <- new TemplatesExportTemplateContext(CSharpContext = csContext)
            t0.Variables <- vars0.ToArray()
            templates.Add t0
          done
        | 2 -> // maybe this is not such a good idea because we get n^2 templates
          for ((tk0,tv0,_),(tk1,tv1,_)) in pairs csharpTypes do
            let t = new TemplatesExportTemplate(shortcut=s+gk+tk0+tk1)
            let vars = List<TemplatesExportTemplateVariable>()
            let genericArgs = gv + "<" + tv0 + "," + tv1 + ">"
            let defValue = "new " + genericArgs + "()"
            t.description <- (printExpressions doc vars defValue).Replace("$typename$", genericArgs)
            t.reformat <- "True"
            t.shortenQualifiedReferences <- "True"
            t.text <- (printExpressions exprs vars defValue).Replace("$typename$", genericArgs)
            t.uid <- newGuid()
            t.Context <- new TemplatesExportTemplateContext(CSharpContext = csContext)
            t.Variables <- vars.ToArray()
            templates.Add t
          done
        | _ -> raise <| new Exception("We don't support this few/many args")
      done
    done

  
  te.Template <- templates.ToArray()

			*/
			const string FILENAME = "ReSharperMnemonics.xml";
			File.Delete(FILENAME);
			var xs = new XmlSerializer(te.GetType());
			using (var fs = new FileStream(FILENAME, FileMode.Create, FileAccess.Write))
			{
				xs.Serialize(fs, te);
			}
			Console.WriteLine(te.Template.Length + "ReSharper templates exported");
		}

		static void Main()
		{
			RenderResharper();
			Thread.Sleep(1000);
		}
	}
}
