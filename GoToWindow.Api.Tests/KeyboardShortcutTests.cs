﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoToWindow.Api.Tests
{
	[TestClass]
	public class KeyboardShortcutTests
	{
		[TestMethod]
		public void ToStringContainsAllKeys()
		{
			// ReSharper disable RedundantArgumentNameForLiteralExpression
			var shortcut = new KeyboardShortcut
			(
				controlVirtualKeyCode: 0xA4,
				virtualKeyCode: 0x09,
				shortcutPressesBeforeOpen: 2
			);
			// ReSharper restore RedundantArgumentNameForLiteralExpression
			Assert.AreEqual("A4+09:2", shortcut.ToString());
		}

		[TestMethod]
		public void FromStringContainsAllOptions()
		{
			var shortcut = KeyboardShortcut.FromString("A4+09:2");
			Assert.AreEqual(0x09, shortcut.VirtualKeyCode);
			Assert.AreEqual(0xA4, shortcut.ControlVirtualKeyCode);
			Assert.AreEqual(2, shortcut.ShortcutPressesBeforeOpen);
		}

		[TestMethod]
		public void FromEmptyStringEnabledIsFalse()
		{
			var shortcut = KeyboardShortcut.FromString("");
			Assert.AreEqual(false, shortcut.Enabled);
		}

		[TestMethod]
		public void CanCreateHumanReadableString()
		{
			Assert.AreEqual("Left Alt + Tab", KeyboardShortcut.FromString("A4+09:1").ToHumanReadableString());
			Assert.AreEqual("Left Alt + Tab + Tab", KeyboardShortcut.FromString("A4+09:2").ToHumanReadableString());
			Assert.AreEqual("Left Win + Tab", KeyboardShortcut.FromString("5B+09:1").ToHumanReadableString());
			Assert.AreEqual("Left Win + Tab + Tab", KeyboardShortcut.FromString("5B+09:2").ToHumanReadableString());
		}
	}
}
