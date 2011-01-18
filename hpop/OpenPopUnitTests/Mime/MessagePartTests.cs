﻿using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using OpenPop.Mime;
using OpenPop.Mime.Header;

namespace OpenPopUnitTests.Mime
{
	[TestFixture]
	public class MessagePartTests
	{
		[Test]
		public void TestIsTextTextPlain()
		{
			const string messagePartContent =
				"Content-Type: text/plain\r\n" +
				"\r\n"; // End of message headers

			MessagePart messagePart = new Message(Encoding.ASCII.GetBytes(messagePartContent)).MessagePart;

			Assert.IsTrue(messagePart.IsText);
		}

		[Test]
		public void TestIsTextTextHtml()
		{
			const string messagePartContent =
				"Content-Type: text/html\r\n" +
				"\r\n"; // End of message headers

			MessagePart messagePart = new Message(Encoding.ASCII.GetBytes(messagePartContent)).MessagePart;

			Assert.IsTrue(messagePart.IsText);
		}

		[Test]
		public void TestIsTextMessageRFC822()
		{
			const string messagePartContent =
				"Content-Type: message/rfc822\r\n" +
				"\r\n"; // End of message headers

			MessagePart messagePart = new Message(Encoding.ASCII.GetBytes(messagePartContent)).MessagePart;

			Assert.IsTrue(messagePart.IsText);
		}

		[Test]
		public void TestIsAttachmentApplicationOgg()
		{
			const string messagePartContent =
				"Content-Type: application/ogg\r\n" +
				"\r\n"; // End of message headers

			MessagePart messagePart = new Message(Encoding.ASCII.GetBytes(messagePartContent)).MessagePart;

			Assert.IsTrue(messagePart.IsAttachment);
		}

		[Test]
		public void TestIsAttachmentApplicationZip()
		{
			const string messagePartContent =
				"Content-Type: application/zip\r\n" +
				"\r\n"; // End of message headers

			MessagePart messagePart = new Message(Encoding.ASCII.GetBytes(messagePartContent)).MessagePart;

			Assert.IsTrue(messagePart.IsAttachment);
		}

		[Test]
		public void TestIsAttachmentApplicationPdf()
		{
			const string messagePartContent =
				"Content-Type: application/pdf\r\n" +
				"\r\n"; // End of message headers

			MessagePart messagePart = new Message(Encoding.ASCII.GetBytes(messagePartContent)).MessagePart;

			Assert.IsTrue(messagePart.IsAttachment);
		}

		[Test]
		public void TestIsAttachmentAudioMpeg()
		{
			const string messagePartContent =
				"Content-Type: audio/mpeg\r\n" +
				"\r\n"; // End of message headers

			MessagePart messagePart = new Message(Encoding.ASCII.GetBytes(messagePartContent)).MessagePart;

			Assert.IsTrue(messagePart.IsAttachment);
		}

		[Test]
		public void TestIsAttachmentImageJpeg()
		{
			const string messagePartContent =
				"Content-Type: image/jpeg\r\n" +
				"\r\n"; // End of message headers

			MessagePart messagePart = new Message(Encoding.ASCII.GetBytes(messagePartContent)).MessagePart;

			Assert.IsTrue(messagePart.IsAttachment);
		}

		[Test]
		public void TestSaveToFile()
		{
			const string base64 =
				"JVBERi0xLjUNCiW1tbW1DQoxIDAgb2JqDQo8PC9UeXBlL0NhdGFsb2cvUGFnZXMgMiAwIFIv\r\n" +
				"TGFuZyhkYS1ESykgL1N0cnVjdFRyZWVSb290IDE1IDAgUi9NYXJrSW5mbzw8L01hcmtlZCB0\r\n" +
				"cnVlPj4+Pg0KZW5kb2JqDQoyIDAgb2JqDQo8PC9UeXBlL1BhZ2VzL0NvdW50IDEvS2lkc1sg\r\n" +
				"MyAwIFJdID4+DQplbmRvYmoNCjMgMCBvYmoNCjw8L1R5cGUvUGFnZS9QYXJlbnQgMiAwIFIv\r\n" +
				"UmVzb3VyY2VzPDwvRm9udDw8L0YxIDUgMCBSL0YyIDcgMCBSL0YzIDkgMCBSPj4vUHJvY1Nl\r\n" +
				"dFsvUERGL1RleHQvSW1hZ2VCL0ltYWdlQy9JbWFnZUldID4+L01lZGlhQm94WyAwIDAgNTk0\r\n" +
				"Ljk2IDg0Mi4wNF0gL0NvbnRlbnRzIDQgMCBSL0dyb3VwPDwvVHlwZS9Hcm91cC9TL1RyYW5z\r\n" +
				"cGFyZW5jeS9DUy9EZXZpY2VSR0I+Pi9UYWJzL1MvU3RydWN0UGFyZW50cyAwPj4NCmVuZG9i";

			const string partPDF =
				"Content-Type: application/pdf;\r\n" +
				" name=\"=?ISO-8859-1?Q?=D8nskeliste=2Epdf?=\"\r\n" +
				"Content-Transfer-Encoding: base64\r\n" +
				"\r\n" +
				base64;

			// Base 64 is only in ASCII
			Message message = new Message(Encoding.ASCII.GetBytes(partPDF));

			MessagePart messagePart = message.MessagePart;

			// Check the headers
			Assert.AreEqual("application/pdf", messagePart.ContentType.MediaType);
			Assert.AreEqual(ContentTransferEncoding.Base64, messagePart.ContentTransferEncoding);
			Assert.AreEqual("Ønskeliste.pdf", messagePart.ContentType.Name);

			byte[] correctBytes = Convert.FromBase64String(base64);
			// This will fail if US-ASCII is assumed on the bytes when decoded from base64 to bytes
			Assert.AreEqual(correctBytes, messagePart.Body);

			FileInfo testFile = new FileInfo("test_message_save_.testFile");
			messagePart.SaveToFile(testFile);

			byte[] fileBytes = File.ReadAllBytes(testFile.ToString());
			testFile.Delete();

			Assert.AreEqual(correctBytes, fileBytes);
		}

		[Test]
		public void TestQuotedPrintableDoesNotDecodeUnderscoresInBody()
		{
			const string messagePartContent =
				"Content-Transfer-Encoding: quoted-printable\r\n" +
				"\r\n" + // End of message headers
				"a_a";

			MessagePart messagePart = new Message(Encoding.ASCII.GetBytes(messagePartContent)).MessagePart;

			// QuotedPrintable, when used as Content-Transfer-Encoding does not decode _ to spaces
			const string expectedBody = "a_a";
			string actualBody = messagePart.GetBodyAsText();

			Assert.AreEqual(expectedBody, actualBody);
		}
	}
}