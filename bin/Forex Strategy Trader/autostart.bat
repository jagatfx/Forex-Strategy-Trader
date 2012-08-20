:: This batch file runs FST automatically, sets connection ID,
:: strategy name to load and starts (or not) autotrade;
:: Example:
:: start "" "Forex Strategy Trader.exe"   100   "yes"   "Test Trade"
:: Where:
::       100 is the ConectionID,
::       "yes" means start autotrade when connected,
::       "no" means do not start autotrading
::       "Test Trade" is the strategy name without extension.
::       timeout 5 sets 5 seconds delay between starts.

start "" "Forex Strategy Trader.exe"   100   "yes"   "Test Trade"
timeout 5
start "" "Forex Strategy Trader.exe"   110   "yes"   "Test Trade"
timeout 5
start "" "Forex Strategy Trader.exe"   120   "yes"   "Test Trade"
timeout 5
start "" "Forex Strategy Trader.exe"   130   "no"    "Test Trade"
timeout 5
start "" "Forex Strategy Trader.exe"   140   "no"    "Test Trade"
