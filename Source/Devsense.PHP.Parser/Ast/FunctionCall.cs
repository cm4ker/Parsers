// Copyright(c) DEVSENSE s.r.o.
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Devsense.PHP.Syntax.Ast
{
    #region FunctionCall

    public abstract class FunctionCall : VarLikeConstructUse
    {
        protected CallSignature callSignature;
        /// <summary>GetUserEntryPoint calling signature</summary>
        public CallSignature CallSignature { get { return callSignature; } set { callSignature = callSignature = value ?? throw new ArgumentNullException(nameof(value)); } }

        /// <summary>
        /// Position of called function name in source code.
        /// </summary>
        public abstract Text.Span NameSpan { get; }

        public FunctionCall(Text.Span span, IList<ActualParam> parameters, Text.Span parametersSpan, IList<TypeRef> genericParams, Text.Span genericParametersSpan)
            : base(span)
        {
            Debug.Assert(parameters != null);

            this.callSignature = new CallSignature(parameters, genericParams, parametersSpan);
        }
    }

    #endregion

    #region DirectFcnCall

    public sealed class DirectFcnCall : FunctionCall
    {
        public override Operations Operation { get { return Operations.DirectCall; } }

        /// <summary>
        /// Complete translated name, contians translated, original and fallback names.
        /// </summary>
        public TranslatedQualifiedName FullName => _fullName;
        readonly TranslatedQualifiedName _fullName;

        /// <summary>Simple name for methods.</summary>
        [Obsolete]
        public QualifiedName QualifiedName => _fullName.Name;

        [Obsolete]
        public QualifiedName? FallbackQualifiedName => _fullName.FallbackName;

        public override Text.Span NameSpan => _fullName.Span;

        public DirectFcnCall(Text.Span span, TranslatedQualifiedName name,
            IList<ActualParam> parameters, Text.Span parametersSpan, IList<TypeRef> genericParams, Text.Span genericParametersSpan)
            : base(span, parameters, parametersSpan, genericParams, genericParametersSpan)
        {
            _fullName = name;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitDirectFcnCall(this);
        }
    }

    #endregion

    #region IndirectFcnCall

    public sealed class IndirectFcnCall : FunctionCall
    {
        public override Operations Operation { get { return Operations.IndirectCall; } }

        public Expression/*!*/ NameExpr { get { return nameExpr; } }
        internal Expression/*!*/ nameExpr;
        public override Text.Span NameSpan => NameExpr.Span;

        public IndirectFcnCall(Text.Span p, Expression/*!*/ nameExpr, IList<ActualParam> parameters, Text.Span parametersSpan, IList<TypeRef> genericParams, Text.Span genericParametersSpan)
            : base(p, parameters, parametersSpan, genericParams, genericParametersSpan)
        {
            this.nameExpr = nameExpr;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIndirectFcnCall(this);
        }
    }

    #endregion

    #region StaticMtdCall

    public abstract class StaticMtdCall : FunctionCall
    {
        public TypeRef TargetType => this.typeRef;
        protected readonly TypeRef/*!*/typeRef;

        /// <summary>
        /// Static method call.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="className">Name of the containing class.</param>
        /// <param name="classNamePosition">Class name position.</param>
        /// <param name="parameters">Actual parameters.</param>
        /// <param name="parametersSpan">Parameters position.</param>
        /// <param name="genericParams">Generic parameters.</param>
        /// <param name="genericParametersSpan">Generic parameters position.</param>
        internal StaticMtdCall(Text.Span span, GenericQualifiedName className, Text.Span classNamePosition, IList<ActualParam> parameters, Text.Span parametersSpan, IList<TypeRef> genericParams, Text.Span genericParametersSpan)
            : this(span, TypeRef.FromGenericQualifiedName(classNamePosition, className), parameters, parametersSpan, genericParams, genericParametersSpan)
        {
        }

        public StaticMtdCall(Text.Span span, TypeRef typeRef, IList<ActualParam> parameters, Text.Span parametersSpan, IList<TypeRef> genericParams, Text.Span genericParametersSpan)
            : base(span, parameters, parametersSpan, genericParams, genericParametersSpan)
        {
            Debug.Assert(typeRef != null);

            this.typeRef = typeRef;
        }
    }

    #endregion

    #region DirectStMtdCall

    public sealed class DirectStMtdCall : StaticMtdCall
    {
        public override Operations Operation { get { return Operations.DirectStaticCall; } }

        private NameRef methodName;
        public NameRef MethodName => methodName;
        public override Text.Span NameSpan => methodName.Span;

        public DirectStMtdCall(Text.Span span, ClassConstUse/*!*/ classConstant,
            IList<ActualParam>/*!*/ parameters, Text.Span parametersSpan, 
            IList<TypeRef>/*!*/ genericParams, Text.Span genericParametersSpan)
            : base(span, classConstant.TargetType, parameters, parametersSpan, genericParams, genericParametersSpan)
        {
            this.methodName = new NameRef(classConstant.NamePosition, classConstant.Name.Value);
        }

        public DirectStMtdCall(Text.Span span, GenericQualifiedName className, Text.Span classNamePosition,
            Name methodName, Text.Span methodNamePosition, IList<ActualParam> parameters, Text.Span parametersSpan, 
            IList<TypeRef> genericParams, Text.Span genericParametersSpan)
            : base(span, className, classNamePosition, parameters, parametersSpan, genericParams, genericParametersSpan)
        {
            this.methodName = new NameRef(methodNamePosition, methodName);
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitDirectStMtdCall(this);
        }
    }

    #endregion

    #region IndirectStMtdCall

    public sealed class IndirectStMtdCall : StaticMtdCall
    {
        public override Operations Operation { get { return Operations.IndirectStaticCall; } }

        /// <summary>Expression that represents name of method.</summary>
        public Expression/*!*/ MethodNameExpression => _methodNameExpr;
        Expression/*!*/_methodNameExpr;

        public override Text.Span NameSpan => _methodNameExpr.Span;


        public IndirectStMtdCall(Text.Span span,
                                 GenericQualifiedName className, Text.Span classNamePosition, Expression/*!*/ nameExpr,
                                 IList<ActualParam> parameters, Text.Span parametersSpan, 
                                 IList<TypeRef> genericParams, Text.Span genericParametersSpan)
            : base(span, className, classNamePosition, parameters, parametersSpan, genericParams, genericParametersSpan)
        {
            _methodNameExpr = nameExpr;
        }

        public IndirectStMtdCall(Text.Span span,
                                 TypeRef/*!*/typeRef, Expression/*!*/ mtdNameVar,
                                 IList<ActualParam> parameters, Text.Span parametersSpan, 
                                 IList<TypeRef> genericParams, Text.Span genericParametersSpan)
            : base(span, typeRef, parameters, parametersSpan, genericParams, genericParametersSpan)
        {
            _methodNameExpr = mtdNameVar;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIndirectStMtdCall(this);
        }
    }

    #endregion
}
