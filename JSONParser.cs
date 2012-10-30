/**

MIT LICENSE

Copyright (C) 2012 - Ryan Ploetz

LinkedIn: http://www.linkedin.com/in/ploetz
Email: rploetz@somnio.com

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
**/

//#define DEBUGGER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JSONParser 
{
	private string json;
	public Dictionary<string, JSONObject> root;

	public JSONParser() 
	{
		json = null;
	}

	public JSONParser(string str) 
	{
		json = null;
		root = new Dictionary<string, JSONObject>();

		if ( str != null )
		{
			str = str.Replace("\\n", "");
			str = str.Replace("\\t", "");
			str = str.Replace("\\r", "");
			str = str.Replace("\t", "");
			str = str.Replace("\n", "");
			str = str.Replace("\\", "");

			if ( str.Length > 0 ) 
			{
				json = str;
			}
		}
	}

	public bool parse()
	{
		if ( json != null )
		{
			parse(root);
			return true;
		}
		return false;
	}

	private int parse(Dictionary<string, JSONObject> pDict)
	{
		if ( json != null )
		{
			int i = 0;
			bool finisher = false;
			for ( i = 0; i < json.Length; i++ )
			{
				switch(json[i])
				{
					case '{':
						finisher = true;
						break;
					case '[':
						finisher = true;
						break;
				}
				if ( finisher )
				{
					break;
				}
			}

			bool openquote = false;
			int first_token = 0;
			int last_token = 0;
			bool inProperty = false;
			string activeKey = "";

			for ( int j = i+1; j < json.Length; j++ )
			{
				if ( json[j] == '"' )
				{
					openquote = !openquote;
					if ( openquote )
					{
						first_token = j + 1;
						#if(DEBUGGER)
							Debug.Log("First Token Set: " + first_token);
						#endif
					}
					else
					{
						last_token = j;
						#if(DEBUGGER)
							Debug.Log("Last Token Set: " + last_token);
						#endif
					}
				}
				if ( json[j] == '[' || json[j] == '{' )
				{
					#if(DEBUGGER)
						Debug.Log("Making new Dictionary...on Key: " + activeKey);
					#endif

					json = json.Substring(j, json.Length-j);
					pDict[activeKey].dict = new Dictionary<string, JSONObject>();
					j = parse(pDict[activeKey].dict) + 1;
					inProperty = false;
					continue;
				}
				if ( !openquote )
				{
					if ( json[j] == ':' && !inProperty)
					{
						JSONObject tempObj = new JSONObject();
						activeKey = json.Substring(first_token, last_token-first_token);
						tempObj.key = activeKey;
						pDict[activeKey] = tempObj;
						inProperty = true;

						#if(DEBUGGER)
							Debug.Log("Key: " + activeKey);
						#endif
					}
					if ( json[j] == ',' )
					{
						pDict[activeKey].value = json.Substring(first_token, last_token-first_token);
						inProperty = false;

						#if(DEBUGGER)
							Debug.Log("Value: " + pDict[activeKey].value);
						#endif
					}
					if ( json[j] == ']' || json[j] == '}' )
					{
						if ( inProperty )
						{
							pDict[activeKey].value = json.Substring(first_token, last_token-first_token);
							inProperty = false;

							#if(DEBUGGER)
								Debug.Log("Value: " + pDict[activeKey].value);
							#endif
						}
						return j;
					}
				}
			}
		}
		return 0;
	}
}