Feature: Invoke-XdtTransform

Scenario: Invoke-XdtTransform with a null SourcePath parameter
    When we invoke Invoke-XdtTransform and the SourcePath parameter is null
	Then a 'System.Management.Automation.CmdletInvocationException' is thrown
    And the inner exception is 'System.ArgumentNullException'
    And the exception message is: 
		"""
		Value cannot be null.
		Parameter name: SourcePath
		"""

Scenario: Invoke-XdtTransform with a source path that does not exist
    When we invoke Invoke-XdtTransform and the SourcePath does not exist
	Then a 'System.Management.Automation.CmdletInvocationException' is thrown
    And the inner exception is 'System.IO.FileNotFoundException'
    And the exception message is: 
		"""
		The source file, 'C:\ThisIsNotARealPath', could not be found.
		"""

Scenario: Invoke-XdtTransform with a null Destination parameter
    When we invoke Invoke-XdtTransform and the Destination parameter is null
	Then a 'System.Management.Automation.CmdletInvocationException' is thrown
    And the inner exception is 'System.ArgumentNullException'
    And the exception message is: 
		"""
		Value cannot be null.
		Parameter name: Destination
		"""

Scenario: Invoke-XdtTransform with a null TransformPath parameter
    When we invoke Invoke-XdtTransform and the TransformPath parameter is null
	Then a 'System.Management.Automation.CmdletInvocationException' is thrown
    And the inner exception is 'System.ArgumentNullException'
    And the exception message is: 
		"""
		Value cannot be null.
		Parameter name: TransformPath
		"""