using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Win32;

namespace CV_Material_Import;

/// <summary>
/// A static class of helper methods.
/// </summary>
internal static class Utilities
{
	public static T GetValueFromRegistry<T>(string keyPath, string valueName)
	{
		object? read;
		if ((read = Registry.GetValue(keyPath, valueName, null)) is null)
		{
			if (typeof(T) == typeof(string))
				SetValueInRegistry(keyPath, valueName, string.Empty);
			else if (typeof(T) == typeof(int))
				SetValueInRegistry(keyPath, valueName, 0);
		}
		return  read is null ? default! : (T)read!;
	}

	public static void SetValueInRegistry<T>(string keyPath, string valueName, T value)
	{
		switch (value)
		{
			case int:
				Registry.SetValue(keyPath, valueName, value, RegistryValueKind.DWord);
				break;
			case string:
				Registry.SetValue(keyPath, valueName, value, RegistryValueKind.String);
				break;
			default:
				Registry.SetValue(keyPath, valueName, value ?? default!);
				break;
		}
	}
}
