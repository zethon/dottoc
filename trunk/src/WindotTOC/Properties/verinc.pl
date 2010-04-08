#!/usr/perl/bin
use strict;
use Data::Dumper;

open FILE, 'AssemblyInfo.cs' or die $!; 
my @lines = <FILE>;
close FILE;

open FILE, '>AssemblyInfo.cs' or die $!; 
foreach my $line (@lines)
{
	if ($line =~ /Version\(\"\d+.\d+.(\d+)\"\)\]/)
	{
		my $ver = $1;
		$ver++;

		$line =~ s/Version\(\"(\d+).(\d+).(\d+)\"\)\]/Version\(\"$1\.$2.$ver\"\)\]/;
	}
	
	print FILE $line;
}
close FILE;