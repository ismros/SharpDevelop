﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Given code:
	/// 
	/// a = Class1()
	/// 
	/// Check that the type of "a" can be obtained by the resolver.
	/// </summary>
	[TestFixture]
	public class ResolveLocalClassInstanceTests
	{
		PythonResolverTestsHelper resolverHelper;
		MockClass testClass;
		
		[SetUp]
		public void Init()
		{
			resolverHelper = new PythonResolverTestsHelper();
			
			testClass = resolverHelper.CreateClass("Test.Test1");
			resolverHelper.ProjectContent.ClassesInProjectContent.Add(testClass);			
			resolverHelper.ProjectContent.SetClassToReturnFromGetClass("Test.Test1", testClass);

		}

		[Test]
		public void Resolve_LocalVariableIsCreatedOnPreviousLine_ResolveResultVariableNameIsA()
		{
			string python =
				"a = Test.Test1()\r\n" +
				"a";
			
			resolverHelper.Resolve("a", python);
			
			string name = resolverHelper.LocalResolveResult.VariableName;
			
			Assert.AreEqual("a", name);
		}
		
		[Test]
		public void Resolve_LocalVariableIsCreatedOnPreviousLine_ResolveResultResolvedTypeIsTestClass()
		{
			string python =
				"a = Test.Test1()\r\n" +
				"a";
			
			resolverHelper.Resolve("a", python);
			
			IReturnType resolvedType = resolverHelper.LocalResolveResult.ResolvedType;
			IClass underlyingClass = resolvedType.GetUnderlyingClass();
			
			Assert.AreEqual(testClass, underlyingClass);
		}
		
		[Test]
		public void Resolve_LocalVariableIsReDefinedAfterLineBeingConsidered_ResolveResultResolvedTypeIsTestClass()
		{
			string python =
				"a = Test.Test1()\r\n" +
				"a\r\n" +
				"a = Unknown.Unknown()\r\n";
			
			ExpressionResult expression = new ExpressionResult("a");
			expression.Region = new DomRegion(
				beginLine: 1,
				beginColumn: 0,
				endLine: 1,
				endColumn: 1);
			
			resolverHelper.Resolve(expression, python);
			
			IReturnType resolvedType = resolverHelper.LocalResolveResult.ResolvedType;
			IClass underlyingClass = resolvedType.GetUnderlyingClass();
			
			Assert.AreEqual(testClass, underlyingClass);
		}
		
		[Test]
		public void Resolve_LocalVariableIsReDefinedAfterLineBeingConsideredAndExpressionRegionEndLineIsMinusOne_ResolveResultResolvedTypeIsTestClass()
		{
			string python =
				"a = Test.Test1()\r\n" +
				"a\r\n" +
				"a = Unknown.Unknown()\r\n";
			
			ExpressionResult expression = new ExpressionResult("a");
			expression.Region = new DomRegion(
				beginLine: 1,
				beginColumn: 0,
				endLine: -1,
				endColumn: 1);
			
			resolverHelper.Resolve(expression, python);
			
			IReturnType resolvedType = resolverHelper.LocalResolveResult.ResolvedType;
			IClass underlyingClass = resolvedType.GetUnderlyingClass();
			
			Assert.AreEqual(testClass, underlyingClass);
		}
	}
}
