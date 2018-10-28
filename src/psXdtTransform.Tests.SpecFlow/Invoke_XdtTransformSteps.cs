using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using TechTalk.SpecFlow;

namespace psXdtTransform.Tests.SpecFlow
{
    [Binding]
    public class Invoke_XdtTransformSteps : IDisposable
    {
        public Invoke_XdtTransformSteps()
        {
            var state = InitialSessionState.CreateDefault();
            state.Commands.Add(
                new SessionStateCmdletEntry(
                    "Invoke-XdtTransform", 
                    typeof(InvokeXdtTransformCommand), 
                    null));


            this._Runspace = RunspaceFactory.CreateRunspace();
            this._Runspace.Open();

            this._Shell = PowerShell.Create(state);            
            this._Shell.Runspace = this._Runspace;

            this._Command = new Command("Invoke-XdtTransform");
            this._Shell.Commands.AddCommand(this._Command);

        }

        private readonly Runspace _Runspace;
        private readonly PowerShell _Shell;
        private readonly Command _Command;

        private Exception _Exception;

        [When(@"we invoke Invoke-XdtTransform and the SourcePath parameter is null")]
        public void WhenWeInvokeInvoke_XdtTransformAndTheSourcePathParameterIsNull()
        {
            Assert.IsFalse(this._Command.Parameters.Any(x => x.Name == nameof(InvokeXdtTransformCommand.SourcePath)));
        }

        [When(@"we invoke Invoke-XdtTransform and the Destination parameter is null")]
        public void WhenWeInvokeInvoke_XdtTransformAndTheDestinationParameterIsNull()
        {
            var path = @"C:\ThisIsNotARealPath";
            this._Command.Parameters.Add(nameof(InvokeXdtTransformCommand.SourcePath), path);
            this._Command.Parameters.Add(nameof(InvokeXdtTransformCommand.TransformPath), path);
            Assert.IsFalse(this._Command.Parameters.Any(x => x.Name == nameof(InvokeXdtTransformCommand.Destination)));

        }

        [When(@"we invoke Invoke-XdtTransform and the TransformPath parameter is null")]
        public void WhenWeInvokeInvoke_XdtTransformAndTheTransformPathParameterIsNull()
        {
            var path = @"C:\ThisIsNotARealPath";
            this._Command.Parameters.Add(nameof(InvokeXdtTransformCommand.SourcePath), path);
            this._Command.Parameters.Add(nameof(InvokeXdtTransformCommand.Destination), path);
            Assert.IsFalse(this._Command.Parameters.Any(x => x.Name == nameof(InvokeXdtTransformCommand.TransformPath)));

        }
        
        [When(@"we invoke Invoke-XdtTransform and the SourcePath does not exist")]
        public void WhenWeInvokeInvoke_XdtTransformAndTheSourcePathDoesNotExist()
        {
            var path = @"C:\ThisIsNotARealPath";
            this._Command.Parameters.Add(nameof(InvokeXdtTransformCommand.SourcePath), path);
            this._Command.Parameters.Add(nameof(InvokeXdtTransformCommand.Destination), path);
            this._Command.Parameters.Add(nameof(InvokeXdtTransformCommand.TransformPath), path);
            Assert.IsFalse(System.IO.Directory.Exists(path));
        }
        
        [Then(@"a '(.*)' is thrown")]
        public void ThenAIsThrown(string p0)
        {
            try
            {
                this._Shell.Invoke().ToList();
            }
            catch (Exception ex)
            {
                this._Exception = ex;
            }
            Assert.IsNotNull(this._Exception);
            Assert.AreEqual(p0, this._Exception.GetType().FullName);
        }

        [Then(@"the inner exception is '(.*)'")]
        public void ThenTheInnerExceptionIs(string p0)
        {
            Assert.IsNotNull(this._Exception.InnerException);
            Assert.AreEqual(p0, this._Exception.InnerException.GetType().FullName);
        }
        
        [Then(@"the exception message is:")]
        public void ThenTheExceptionMessageIs(string multilineText)
        {
            Assert.AreEqual(multilineText, this._Exception.Message);
        }

        public void Dispose()
        {
            this._Shell?.Dispose();
            this._Runspace?.Dispose();
        }
    }
}
