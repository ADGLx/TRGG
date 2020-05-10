using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CosaAle : MonoBehaviour
{

    public TMP_Text TextM;
    private string Text;
    public TMP_Text TextOutput;
    Dictionary<string,IDictionary<Llaves, string>> Output = new Dictionary<string, IDictionary<Llaves, string>>(); //A dictionary that has as a key the email and inside of it contains a dictionary with all the other stuff

    void Start()
    {
        Text = TextM.text;

     //   Debug.Log(Text[0]);
    }


    public void GenerateOutput()
    {
        List<String> ListNames = new List<string>();
            List<String> ListEmails = new List<string>();
 
        for (int X = 0; X < Text.Length; X++) //first generate the amount of dictionaries per each email
        {
            if(Text[X] == '@') //An @ was found 
            {
                string emailTemp = "";

                for(int y = 0; y< Text.Length; y++) //getting the first part of the email
                {
                    if (Text[X - y] == ' ')
                        break;


                    emailTemp += Text[X - y];
                }

                string invertTemp ="";
                for(int w = emailTemp.Length -1; w>=0;w--) //inverting the string
                {
                    invertTemp += emailTemp[w];
                }
                emailTemp = invertTemp;

                string emailTemp2 = "";
                for(int z = X + 1; z < Text.Length;z++) //aqui esto se puede romper pero me da caliweba arreglarlo
                {
                    emailTemp2 += Text[z];

                    if (Text[z] == 'm' && Text[z - 1] == 'o' && Text[z - 2] == 'c')
                        break;
                }
                emailTemp += emailTemp2;

                ListEmails.Add(emailTemp);
              //  Debug.Log(emailTemp); email done
            }

            if (Char.IsUpper(Text[X]) && X> 1) //Esto es muy ghetto pero me dio lala buscar otra manera para que no saliera el yo
            {
                string TempName = "";
                bool FoundName = false;
                TempName += Text[X];
                for (int m = X + 1 ; m < Text.Length; m++)
                {
                    TempName += Text[m];

                    if (Text[m] == ' ')
                    {
                        if (m + 1 <= Text.Length && Char.IsUpper(Text[m + 1]))
                        {
                            FoundName = true;

                            for (int n = m + 1; n < Text.Length; n++)
                            {
                                if (n != m && !Char.IsLetter(Text[n]) && !Char.IsUpper(Text[n + 1]))
                                {
                                    X = n; // se salte el segundo nombre ahora, capaz y rompa vainas
                                    break;
                                }
                                TempName += Text[n];
                            }
                        } 
                        break;      
                    }
                    else
                    {
                        FoundName = false;
                    }
                }

                if(FoundName)
                {
                    //Debug.Log(TempName);
                    ListNames.Add(TempName);
                }
                      
            }
        }

        if (ListEmails.Count == ListNames.Count)
        {
            for(int x = 0; x < ListEmails.Count; x++) // Lazy way because theu are in order
            {
                string N2 = ListEmails[x];
                string N = ListNames[x];
                Dictionary<Llaves, string> t = new Dictionary<Llaves, string>();
                Output.Add(N, t);
                Output[N].Add(Llaves.name, N);
                Output[N].Add(Llaves.email, N2);

            }
            PrintToScreen();
        } 
        else 
        {
            Debug.LogWarning("Error");
             }



    }

    private void PrintToScreen()
    {
        string OutputTemp = "";

        foreach(string K in Output.Keys)
        {
            OutputTemp += " Name: " + Output[K][Llaves.name] + " | Email: " + Output[K][Llaves.email] + "\n";
        }

        TextOutput.text = OutputTemp;
    }

}

 enum Llaves {name, email, profesion};
