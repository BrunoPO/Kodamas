using System;
using System.Text;
using UnityEngine;

public class Convert{
	static char[] baseChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray ();

	static public string IPToHash(string address){
		uint val = IP2Long (address);
		string encodedasBASE62 = IntToString (val);
		return encodedasBASE62;
	}

	static public string HashToIP(string encodedasBASE62){
		long teste = StringToInt (encodedasBASE62);
		return Long2IP (teste);
	}

	static private string Long2IP(long longIP){
		string ip = string.Empty;
		for (int i = 0; i < 4; i++){
			int num = (int)(longIP / Mathf.Pow(256, (3 - i)));
			longIP = longIP - (long)(num * Mathf.Pow(256, (3 - i)));
			if (i == 0)
				ip = num.ToString();
			else
				ip  = ip + "." + num.ToString();
		}
		return ip;
	}

	static private uint IP2Long(string ip){
		string[] ipBytes; 
		double num = 0;
		if(!string.IsNullOrEmpty(ip)) {
			ipBytes = ip.Split('.');
			for (int i = ipBytes.Length - 1; i >= 0; i--){
				num += ((int.Parse(ipBytes[i]) % 256) * Mathf.Pow(256, (3 - i)));
			}
		}
		return (uint)num;
	}

	static private string IntToString(uint value){
		string result = string.Empty;
		uint targetBase =(uint) baseChars.Length-1;

		do
		{
			result = baseChars[value % targetBase] + result;
			value = value / targetBase;
		} 
		while (value > 0);
		return result;
	}

	static private long StringToInt(string encodedString){
		double result = 0;
		int sourceBase = baseChars.Length;
		int nextCharIndex = 0;

		for (int currentChar = encodedString.Length - 1; currentChar >= 0; currentChar--)
		{
			char next = encodedString[currentChar];
			for (nextCharIndex = 0; nextCharIndex < baseChars.Length; nextCharIndex++)
			{
				if (baseChars[nextCharIndex] == next)
				{
					break;
				}
			}
			result += Math.Pow(baseChars.Length-1, encodedString.Length - 1 - currentChar) * nextCharIndex;
		}
		return (long)result;
	}
}

