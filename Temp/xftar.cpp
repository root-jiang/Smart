/*
 * XFTA version 1.1
 * xftar
 * Copyrights (c) 2012 Antoine Rauzy
 */

/*
 * Table of Contents
 * -----------------
 * 1) Include Files
 * 2) Main
 */

/*
 * 1) Include Files
 * ----------------
 */

#include <stdio.h>
#include <stdlib.h>
#include "xfta-api.h"

/*
 * 2) Main
 * -------
 */

typedef int (*XFTACommand)(char*);

         /*---------*/

int main(int argc, char** argv)
{
  XFTACommand evalScriptFile = 0;
  int         error = 0;

  #ifdef __WINDOWS__
    HINSTANCE hDLL;
    hDLL = LoadLibrary("xfta.dll");
    if (hDLL==NULL) {
      fprintf(stderr,"No xfta DLL\n"); fflush(stderr);
      exit(1);
      }
    evalScriptFile = (XFTACommand) GetProcAddress(hDLL,"XFTA_EvalScriptFile");
    if (evalScriptFile==0) {
      fprintf(stderr,"No function XFTA_EvalScriptFile\n"); fflush(stderr);
      exit(1);
      }
  #endif

  #ifdef __UNIX__
    evalScriptFile = XFTA_EvalScriptFile;
  #endif

  for (int i=1; i<argc && error==0; i++)
    error = evalScriptFile(argv[i]);

  #ifdef __WINDOWS__
    FreeLibrary(hDLL);
  #endif

  printf("bye\n");
  fflush(stdout);
  return(error);
}

         /*---------*/



