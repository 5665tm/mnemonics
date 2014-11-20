module CSharp

open Types

let csContext = 
  new TemplatesExportTemplateContextCSharpContext (
    context = "TypeMember, TypeAndNamespace",
    minimumLanguageVersion = 2.0M
  )

let csharpTypes =
  [
    ("b", "bool", "false", false)
    (*"c", "char", "0"*)
    ("f", "float", "0.0f", false)
    ("y", "byte", "0", false)
    ("d", "double", "0.0", false)
    ("i", "int", "0", false)
    (*"dec", "decimal", "0M"*)
    ("s", "string", "\"\"", false)
    (*"l", "long", "0"*)
    (*"u", "uint", "0"*)
    (*"g", "System.Guid", "System.Guid.NewGuid()"*)
    ("t", "Transform", "new Transform", true)
    ("g", "GameObject", "new GameObject", true)
    (*"m", "Material", "new Material"*)
    (*"a", "AudioClip", "new AudioClip"*)
    (*"sb", "System.Text.StringBuilder", "new System.Text.StringBuilder"*)
    (*"dt", "System.DateTime", "System.DateTime.UtcNow"*)
  ]

let cSharpStructureTemplates =
  [
    (
      "c",
      [
        Text "public class "
        Constant ("CLASSNAME", "MyClass")
        Scope [
          endConstant
        ]
      ]
    )
//    (
//      "a",
//      [
//        Text "public abstract class "
//        Constant ("CLASSNAME", "MyClass")
//        Scope [
//          endConstant
//        ]
//      ]
//    )
    (
      "C",
      [
        Text "public static class "
        Constant ("CLASSNAME", "MyClass")
        Scope [
          endConstant
        ]
      ]
    )
//    (
//      "i",
//      [
//        Text "public interface "
//        Constant ("INTERFACENAME", "IMyInterface")
//        Scope [ endConstant ]
//      ]
//    )
//    (
//      "s",
//      [
//        Text "public struct "
//        Constant ("STRUCTNAME", "MyStruct")
//        Scope [ endConstant ]
//      ]
//    )
    (
      "e",
      [
        Text "public enum "
        Constant ("ENUMNAME", "MyEnum")
        Scope [ endConstant ]
      ]
    )
  ]

let cSharpMemberTemplates =
  [
//    (
//      "vr",
//      [
//        Text "A readonly field of type "
//        FixedType
//      ],
//      [
//        Text "private readonly "
//        Constant ("type", "type")
//        Text " "
//        Constant ("fieldname", "fieldname")
//        semiColon
//      ]
//    )

// инициализированные переменные
    (
      "v",
      [
        Text "A field of type "
        FixedType
        Text " initialized to the default value."
      ],
      [
        Text "private "
        FixedType
        Text " _"
        Constant ("fieldname", "fieldname")
        Text " = "
        DefaultVariable(DefaultValue)
        semiColon
      ]
    )
    (
      "V",
      [
        Text "A static field of type "
        FixedType
        Text " initialized to the default value."
      ],
      [
        Text "private static "
        FixedType
        Text " _"
        Constant ("fieldname", "fieldname")
        Text " = "
        DefaultVariable(DefaultValue)
        semiColon
      ]
    )
    (
      "pv",
      [
        Text "A public field of type "
        FixedType
        Text " initialized to the default value."
      ],
      [
        Text "public "
        FixedType
        Text " "
        Constant ("fieldname", "fieldname")
        Text " = "
        DefaultVariable(DefaultValue)
        semiColon
      ]
    )
    (
      "pV",
      [
        Text "A public static field of type "
        FixedType
        Text " initialized to the default value."
      ],
      [
        Text "public static "
        FixedType
        Text " "
        Constant ("fieldname", "fieldname")
        Text " = "
        DefaultVariable(DefaultValue)
        semiColon
      ]
    )

    // переменные без инициализации
    (
      "nv",
      [
        Text "A field of type "
        FixedType
      ],
      [
        Text "private "
        FixedType
        Text " _"
        Constant ("fieldname", "fieldname")
        semiColon
      ]
    )
    (
      "nV",
      [
        Text "A static field of type "
        FixedType
      ],
      [
        Text "private static "
        FixedType
        Text " _"
        Constant ("fieldname", "fieldname")
        semiColon
      ]
    )
    (
      "npv",
      [
        Text "A public field of type "
        FixedType
      ],
      [
        Text "public "
        FixedType
        Text " "
        Constant ("fieldname", "fieldname")
        semiColon
      ]
    )
    (
      "npV",
      [
        Text "A public static field of type "
        FixedType
      ],
      [
        Text "public static "
        FixedType
        Text " "
        Constant ("fieldname", "fieldname")
        semiColon
      ]
    )

//    (
//      "n",
//      [
//        Text "A field of type "
//        FixedType
//        Text " initialized to the default value."
//      ],
//      [
//        Text "private "
//        FixedType
//        Text " "
//        Constant ("fieldname", "fieldname")
//        Text " = "
////        Constant("defValue", DefaultValue)
//        DefaultVariable(DefaultValue)
//        semiColon
//      ]
//    )
//    (
//      "o",
//      [
//        Text "A readonly field of type "
//        FixedType
//        Text " initialized to the default value."
//      ],
//      [
//        Text "private readonly "
//        FixedType
//        Text " "
//        Constant ("fieldname", "fieldname")
//        Text " = "
//        DefaultValue
//        semiColon
//      ]
//    )
//    (
//      "t",
//      [
//        Text "A test method."
//      ],
//      [
//        Text "[Test] public void "
//        Constant ("methodname", "MyMethod")
//        Text "()"
//        Scope [
//          endConstant
//        ]
//      ]
//    )
    (
      "m",
      [
        Text "A method that returns a(n) "
        FixedType
      ],
      [
        Text "private "
        space
        FixedType
        space
        Constant ("methodname", "MyMethod")
        Text "()"
        Scope [
          endConstant
        ]
      ]
    )
    (
      "M",
      [
        Text "A static method that returns a(n) "
        FixedType
      ],
      [
        Text "private static "
        FixedType
        space
        Constant ("methodname", "MyMethod")
        Text "()"
        Scope [
          endConstant
        ]
      ]
    )
    (
      "pm",
      [
        Text "A method that returns a(n) "
        FixedType
      ],
      [
        Text "public "
        space
        FixedType
        space
        Constant ("methodname", "MyMethod")
        Text "()"
        Scope [
          endConstant
        ]
      ]
    )
    (
      "pM",
      [
        Text "A static method that returns a(n) "
        FixedType
      ],
      [
        Text "public static "
        FixedType
        space
        Constant ("methodname", "MyMethod")
        Text "()"
        Scope [
          endConstant
        ]
      ]
    )
//    (
//      "p",
//      [
//        Text "An automatic property of type "
//        FixedType
//      ],
//      [
//        Text "public "
//        FixedType
//        space
//        Constant("propname", "MyProperty")
//        Text "{ get; set; }"
//        endConstant
//      ]
//    )
//    (
//      "pr",
//      [
//        Text "An automatic property of type "
//        FixedType
//        Text " with a private setter"
//      ],
//      [
//        Text "public "
//        FixedType
//        space
//        Constant("propname", "MyProperty")
//        Text "{ get; private set; }"
//        endConstant
//      ]
//    )
//    (
//      "pg",
//      [
//        Text "An automatic property of type "
//        FixedType
//        Text " with an empty getter and no setter"
//      ],
//      [
//        Text "public "
//        FixedType
//        Text " "
//        Constant("propname", "MyProperty")
//        Scope [
//          Text "get "
//          Scope [ endConstant ]
//        ]
//      ]
//    )
//    (
//      "d",
//      [
//        Text "A dependency property of type "
//        FixedType
//        Text "."
//      ],
//      [
//        Text "public "
//        FixedType
//        Text " "
//        Constant("propname", "MyProperty")
//        Scope [
//          Text "get "
//          Scope [
//            Text "return ("
//            FixedType
//            Text ")GetValue("
//            Constant("propname", "MyProperty")
//            Text "Property);"
//          ]
//          Text "set "
//          Scope [
//            Text "SetValue("
//            Constant("propname", "MyProperty")
//            Text "Property, value);"
//          ]
//        ]
//        Text "public static readonly System.Windows.DependencyProperty "
//        Constant("propname", "MyProperty")
//        Text "Property ="
//      ]
//    )
  ]