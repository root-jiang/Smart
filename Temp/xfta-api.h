/*
 * XFTA Version 1.1
 * xfta-api.h
 * Copyrights (c) 2012 Antoine Rauzy
 */

#ifndef __XFTA_API__
#define __XFTA_API__

#ifdef __WINDOWS__
  #include <windows.h>
  #include <process.h>
  #ifndef _WINDLL
    #define _WINDLL
  #endif
  #ifdef _WINDLL
    #define DLLEXPORT extern "C" __declspec(dllexport)
    #define DLLIMPORT __declspec(dllimport)
  #else
    #define DLLEXPORT
    #define DLLIMPORT
  #endif
#else
  #define DLLEXPORT
  #define DLLIMPORT
#endif

DLLEXPORT int XFTA_EvalScriptFile(char* fileName);

#endif

