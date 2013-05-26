// MT4 Errors
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

namespace MT4Bridge
{
    public static class MT4Errors
    {
        /// <summary>
        /// Return error description
        /// </summary>
        public static string ErrorDescription(int errorCode)
        {
           string errorString;

           switch(errorCode)
           {
              //---- codes returned from trade server
              case 0:
              case 1:   errorString="No error";                                                  break;
              case 2:   errorString="Common error";                                              break;
              case 3:   errorString="Invalid trade parameters";                                  break;
              case 4:   errorString="Trade server is busy";                                      break;
              case 5:   errorString="Old version of the client terminal";                        break;
              case 6:   errorString="No connection with trade server";                           break;
              case 7:   errorString="Not enough rights";                                         break;
              case 8:   errorString="Too frequent requests";                                     break;
              case 9:   errorString="Malfunctional trade operation (never returned error)";      break;
              case 64:  errorString="Account disabled";                                          break;
              case 65:  errorString="Invalid account";                                           break;
              case 128: errorString="Trade timeout";                                             break;
              case 129: errorString="Invalid price";                                             break;
              case 130: errorString="Invalid stops";                                             break;
              case 131: errorString="Invalid trade volume";                                      break;
              case 132: errorString="Market is closed";                                          break;
              case 133: errorString="Trade is disabled";                                         break;
              case 134: errorString="Not enough money";                                          break;
              case 135: errorString="Price changed";                                             break;
              case 136: errorString="Off quotes";                                                break;
              case 137: errorString="Broker is busy (never returned error)";                     break;
              case 138: errorString="Requote";                                                   break;
              case 139: errorString="Order is locked";                                           break;
              case 140: errorString="Long positions only allowed";                               break;
              case 141: errorString="Too many requests";                                         break;
              case 145: errorString="Modification denied because order too close to market";     break;
              case 146: errorString="Trade context is busy";                                     break;
              case 147: errorString="Expirations are denied by broker";                          break;
              case 148: errorString="Amount of open and pending orders has reached the limit";   break;
              case 149: errorString= "Opening of an opposite position (hedging) is disabled";    break;
              case 150: errorString= "An attempt to close a position contravening the FIFO rule";break; 
              //---- mql4 errors
              case 4000: errorString="No error (never generated code)";                          break;
              case 4001: errorString="Wrong function pointer";                                   break;
              case 4002: errorString="Array index is out of range";                              break;
              case 4003: errorString="No memory for function call stack";                        break;
              case 4004: errorString="Recursive stack overflow";                                 break;
              case 4005: errorString="Not enough stack for parameter";                           break;
              case 4006: errorString="No memory for parameter string";                           break;
              case 4007: errorString="No memory for temp string";                                break;
              case 4008: errorString="Not initialized string";                                   break;
              case 4009: errorString="Not initialized string in array";                          break;
              case 4010: errorString="No memory for array\' string";                             break;
              case 4011: errorString="Too long string";                                          break;
              case 4012: errorString="Remainder from zero divide";                               break;
              case 4013: errorString="Zero divide";                                              break;
              case 4014: errorString="Unknown command";                                          break;
              case 4015: errorString="Wrong jump (never generated error)";                       break;
              case 4016: errorString="Not initialized array";                                    break;
              case 4017: errorString="Dll calls are not allowed";                                break;
              case 4018: errorString="Cannot load library";                                      break;
              case 4019: errorString="Cannot call function";                                     break;
              case 4020: errorString="Expert function calls are not allowed";                    break;
              case 4021: errorString="Not enough memory for temp string returned from function"; break;
              case 4022: errorString="System is busy (never generated error)";                   break;
              case 4050: errorString="Invalid function parameters count";                        break;
              case 4051: errorString="Invalid function parameter value";                         break;
              case 4052: errorString="String function internal error";                           break;
              case 4053: errorString="Some array error";                                         break;
              case 4054: errorString="Incorrect series array using";                             break;
              case 4055: errorString="Custom indicator error";                                   break;
              case 4056: errorString="Arrays are incompatible";                                  break;
              case 4057: errorString="Global variables processing error";                        break;
              case 4058: errorString="Global variable not found";                                break;
              case 4059: errorString="Function is not allowed in testing mode";                  break;
              case 4060: errorString="Function is not confirmed";                                break;
              case 4061: errorString="Send mail error";                                          break;
              case 4062: errorString="String parameter expected";                                break;
              case 4063: errorString="Integer parameter expected";                               break;
              case 4064: errorString="Double parameter expected";                                break;
              case 4065: errorString="Array as parameter expected";                              break;
              case 4066: errorString="Requested history data in update state";                   break;
              case 4099: errorString="End of file";                                              break;
              case 4100: errorString="Some file error";                                          break;
              case 4101: errorString="Wrong file name";                                          break;
              case 4102: errorString="Too many opened files";                                    break;
              case 4103: errorString="Cannot open file";                                         break;
              case 4104: errorString="Incompatible access to a file";                            break;
              case 4105: errorString="No order selected";                                        break;
              case 4106: errorString="Unknown symbol";                                           break;
              case 4107: errorString="Invalid price parameter for trade function";               break;
              case 4108: errorString="Invalid ticket";                                           break;
              case 4109: errorString="Trade is not allowed in the expert properties";            break;
              case 4110: errorString="Longs are not allowed in the expert properties";           break;
              case 4111: errorString="Shorts are not allowed in the expert properties";          break;
              case 4200: errorString="Object is already exist";                                  break;
              case 4201: errorString="Unknown object property";                                  break;
              case 4202: errorString="Object is not exist";                                      break;
              case 4203: errorString="Unknown object type";                                      break;
              case 4204: errorString="No object name";                                           break;
              case 4205: errorString="Object coordinates error";                                 break;
              case 4206: errorString="No specified subwindow";                                   break;
              default:   errorString="Unknown error";                                            break;
          }

            return errorString;
       }
    }
}
