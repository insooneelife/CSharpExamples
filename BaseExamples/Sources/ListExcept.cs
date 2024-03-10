using System;
using System.Collections.Generic;
using System.Linq;

class ListExcept
{
	public static void Example()
	{
		// Define the first list
		List<int> list1 = new List<int> {  3, 4, 5, 1, 2 };

		// Define the second list
		List<int> list2 = new List<int> { 7, 3, 5, 6, 4 };

		// Use Except to exclude elements from list1 that are in list2
		List<int> resultList = list1.Except(list2).ToList();

		// Display the result
		Console.WriteLine("Elements in list1 not in list2:");
		foreach (int value in resultList)
		{
			Console.WriteLine(value);
		}
	}
}