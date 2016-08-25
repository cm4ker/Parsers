<?php  

new MyClass(1, "hello");
    
/** class declaration */
$x = new class(1, "world")
{
    
    private static $var = 'a default value' /** property declaration */, 
            $vara = 10 /** second doc */;
                    
    /** default property declaration */     
    var $pvar = 1; 
                    
    /** constant declaration */     
    public const cvar = 22, cvar = 10;
    
    final $varabc /** second doc */;
}
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NewEx":{
    "DirectTypeRef":{"ClassName":"MyClass"},
    "ActualParam":{"IsUnpack":"False","LongIntLiteral":{"Value":"1"}},
    "ActualParam":{"IsUnpack":"False","StringLiteral":{"Value":"hello"}}
  },
  "ValueAssignEx":{
    "Operation":"AssignValue",
    "DirectVarUse":{"VarName":"x"},
    "NewEx":{
      "AnonymousTypeDecl":{
        "MemberAttributes":"Public",
        "IsConditional":"True",
        "PHPDoc":{"Comment":"classdeclaration"},
        "FieldDeclList":{
          "FieldDecl":{"Name":"var","MemberAttributes":"Error","PHPDoc":{"Comment":"propertydeclaration"},"StringLiteral":{"Value":"adefaultvalue"}},
          "FieldDecl":{"Name":"vara","MemberAttributes":"Error","PHPDoc":{"Comment":"seconddoc"},"LongIntLiteral":{"Value":"10"}}
        },
        "FieldDeclList":{
          "PHPDoc":{"Comment":"defaultpropertydeclaration"},
          "FieldDecl":{"Name":"pvar","MemberAttributes":"Public","PHPDoc":{"Comment":"defaultpropertydeclaration"},"LongIntLiteral":{"Value":"1"}}
        },
        "ConstDeclList":{
          "PHPDoc":{"Comment":"constantdeclaration"},
          "ClassConstantDecl":{"Name":"cvar","MemberAttributes":"Public","PHPDoc":{"Comment":"constantdeclaration"},"LongIntLiteral":{"Value":"22"}},
          "ClassConstantDecl":{"Name":"cvar","MemberAttributes":"Public","LongIntLiteral":{"Value":"10"}}
        },
        "FieldDeclList":{
          "FieldDecl":{"Name":"varabc","MemberAttributes":"Final","PHPDoc":{"Comment":"seconddoc"}}
        }
      },
      "ActualParam":{"IsUnpack":"False","LongIntLiteral":{"Value":"1"}},
      "ActualParam":{"IsUnpack":"False","StringLiteral":{"Value":"world"}}
    }
  } 
}
