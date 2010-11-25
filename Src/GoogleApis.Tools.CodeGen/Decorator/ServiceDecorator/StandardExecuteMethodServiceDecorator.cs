using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Google.Apis.Tools.CodeGen.Decorator.ServiceDecorator {
	public class StandardExecuteMethodServiceDecorator : IServiceDecorator {
		private static log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(StandardExecuteMethodServiceDecorator));
		
		public void DecorateClass (Google.Apis.Discovery.IService service, System.CodeDom.CodeTypeDeclaration serviceClass)
		{
			Logger.Debug("Entering DecorateClass");
			serviceClass.Members.Add(CreateExecuteRequestMethod());
		}
		
		private CodeTypeMember CreateExecuteRequestMethod(){
			var method = new CodeMemberMethod();
			
			method.Name = "ExecuteRequest";
			method.ReturnType = new CodeTypeReference(typeof(System.IO.Stream));
			method.Attributes = MemberAttributes.Public;
			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string),"resource"));
			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string),"method"));
			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string),"body"));
			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IDictionary<string,string>),"parameters"));
			
			//Google.Apis.Requests.Request request = this.genericService.CreateRequest(resource, method);
			method.Statements.Add(CreateRequestLocalVar());
			
			
			
			// return request
			//		.WithAuthentication(authentication)
			//	    .WithParameters(parameterDictionary)
			//		.WithBody(bodyString)
			//		.ExecuteRequest()
			
			//request.WithParameters(parameters)
			var methodInvoke = new CodeMethodInvokeExpression(
								new CodeMethodReferenceExpression(
			                        new CodeVariableReferenceExpression("request"),
					    			"WithParameters"),
					    		new CodeVariableReferenceExpression("parameters"));
			
			methodInvoke = new CodeMethodInvokeExpression(
					    new CodeMethodReferenceExpression(methodInvoke,"WithAuthentication"),
					    new CodeVariableReferenceExpression("authenticator"));
			
			//.WithBody(body)
			methodInvoke = new CodeMethodInvokeExpression(
					    new CodeMethodReferenceExpression(methodInvoke,"WithBody"),
					    new CodeVariableReferenceExpression("body"));
			//.ExecuteRequest()	
			methodInvoke = new CodeMethodInvokeExpression(
			    new CodeMethodReferenceExpression(methodInvoke, "ExecuteRequest"));
			var returnStatment = new CodeMethodReturnStatement(methodInvoke);
			 
			method.Statements.Add(returnStatment);
			                                                                                             
			
			return method;
		}
		
		/// <summary>
		/// <code>Google.Apis.Requests.Request request = this.genericService.CreateRequest(resource, method);</code>
		/// </summary>
		private CodeVariableDeclarationStatement CreateRequestLocalVar(){
			var createRequest = new CodeMethodInvokeExpression();
			createRequest.Method = new CodeMethodReferenceExpression(
			                         new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), ServiceClassGenerator.GENERIC_SERVICE_NAME),
			                         "CreateRequest");
			createRequest.Parameters.Add(new CodeVariableReferenceExpression("resource"));
			createRequest.Parameters.Add(new CodeVariableReferenceExpression("method"));
			
			var createAndAssignRequest = new CodeVariableDeclarationStatement();
			createAndAssignRequest.Type = new CodeTypeReference(typeof(Requests.Request));
			createAndAssignRequest.Name = "request";
			createAndAssignRequest.InitExpression = createRequest;
			
			return createAndAssignRequest;
		}
	}
}
