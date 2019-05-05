#!/bin/bash

mkdir tmp
convert raw/*walk*.png -crop 16x32 tmp/frame.png

sheets=""
for i in {0..3}
do
	startingFrame=`expr $i \* 32`
   	for k in {0..3}
	do
		frame0=`expr $startingFrame + $k + 0 \* 8`
		frame1=`expr $startingFrame + $k + 1 \* 8`
		frame2=`expr $startingFrame + $k + 2 \* 8`
		frame3=`expr $startingFrame + $k + 3 \* 8`
		sheetNumber=`expr $i \* 4 + $k`
		montage "tmp/frame-$frame0.png" "tmp/frame-$frame1.png" "tmp/frame-$frame2.png" "tmp/frame-$frame3.png" -tile x1 -geometry 16x32+0+0 "tmp/sheet-$sheetNumber.png"
		sheets="$sheets tmp/sheet-$sheetNumber.png"
	done
done 
convert -loop 0 -delay 20 $sheets tmp/four-characters.gif

convert tmp/four-characters.gif -coalesce tmp/four-characters-coalesced.gif
convert -size 64x32 tmp/four-characters-coalesced.gif -resize 512x256 final/four-characters.gif
rm -r tmp