#!/bin/bash

##############################################################################
# This script calculate the version which contains the following format
#    <Human readable semantic version>-<build Id>-<dirty flag><short git commit hash>   : e.g. "1.0.1-2169-gd3d4be7"

#  Inputs
#    -  <Human readable semantic version> :      E.g. "1.2.3"
#                Currently, the version is part of this script
#    -  <build Id>                        :   E.g.  "752" 
#                This will come from the build system, it will be injected into the script via environment variable `ENV_BUILD_ID`
#                Refer: https://learn.microsoft.com/en-us/azure/devops/pipelines/process/run-number
#
#
#  Calculated values
#    -  <dirty flag>                      :   E.g.  "d" for dirty, "g" if it's clean
#             It defines if any files have changed. This can be calculated using git command
#    -  <short git commit hash>           :   E.g.  "d3f4db4"
#             This can be calculated using the git command (`git rev-parse --short HEAD`)
##############################################################################

#define default values
human_readable_semantic_version="0.0.1"
default_buildId="D"  # 'D' represents it build locally on developer machine not from CI system

# This final output version
version_str=""

#define output files
versionfile="autoversion.json"

get_version()
{
    # Add human readable version
    [[ ! -z "${human_readable_semantic_version}" ]] && version_str="${human_readable_semantic_version}"
    
    #Add build id from environment variable `ENV_BUILD_ID`
    [[ -z "${ENV_BUILD_ID}" ]] && buildId="${default_buildId}" || buildId="${ENV_BUILD_ID}"
    version_str="${version_str}-${buildId}"

    # Add dirty/clean flag
    git diff --quiet && dirty_flag="g" || dirty_flag="d"
    version_str="${version_str}-${dirty_flag}"

    # Add short commit hash
    short_commit_hash=$(git rev-parse --short HEAD)
    version_str="${version_str}${short_commit_hash}"
}

write_version_json_file()
{
    #remove any old version files
    rm -f $versionfile

    # Create file content
    jsonFile='{ "Version": "'${version_str}'" }'

    # write file
    echo -n $jsonFile >> $versionfile
}

# Get version
get_version
echo $version_str

# Write version to a json file
write_version_json_file
exit 0

