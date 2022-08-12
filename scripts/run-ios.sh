#!/usr/bin/env bash

set -euo pipefail

# NOTE(jpr): this is adapted from the following comments
#   https://github.com/dotnet/xamarin/issues/26#issuecomment-757981580
#   https://github.com/dotnet/maui/discussions/2364#discussioncomment-1286275

# Config

readonly _tfm="net6.0-ios"

# Parameter parsing

readonly _app_name="${1:-}"
readonly _mode="${2:-}"
readonly _provided_udid="${3:-}"

if [ -z "${_app_name}" ]; then
  echo "ERROR: you must provide your app name"
  exit 1
fi

case "${_mode}" in
  "sim" | "device")
    ;;
  "")
    echo "ERROR: no mode provided. It must be one of [sim, device]"
    exit 1
    ;;
  *)
    echo "ERROR: invalid mode [${_mode}]. It must be one of [sim, device]"
    exit 1
    ;;
esac

# DO WORK SON

readonly cur_dir="$( dirname $0 )"

list_ios_devices() {
  xcrun xctrace list devices
}

list_ios_sims() {
  xcrun simctl list
}

get_current_udid() {
  local -r mode="${1:-}"

  case "${mode}" in
    "sim")
      list_ios_sims | grep -i booted | \
        head -1 | grep -Eo '\([A-F0-9]+(-[A-F0-9]+){1,}\)' | sed 's/[()]//g'
      ;;
    "device")
      list_ios_devices | awk '/== Devices ==/{flag=1;next}/== Simulators ==/{flag=0}flag' | grep -v $( hostname ) | \
        head -1 | grep -Eo '\([A-F0-9]+(-[A-F0-9]+){1,}\)' | sed 's/[()]//g'
      ;;
  esac
}

get_device_name_for_udid() {
  local -r udid="${1:-}"
  local -r mode="${2:-}"

  case "${mode}" in
    "sim")
      list_ios_sims | grep -Eo ".+ \(${udid}\)" | sed "s/ (${udid})//" | xargs
      ;;
    "device")
      list_ios_devices | grep -Eo ".+ \(${udid}\)" | sed "s/ (${udid})//" | xargs
      ;;
  esac
}

main() {
  local -r app_name="${1:-}"
  local -r mode="${2:-}"
  local udid="${3:-}"

  if [ -z "${udid}" ]; then
    echo "No udid provided... trying to read the currently running ${mode}"
    local udid="$( get_current_udid "${mode}" )"

    if [ -z "${udid}" ]; then
      echo "ERROR: no running ${mode} found. Please pass a UDID manually."
      exit 1
    fi
  fi

  local -r device_name="$( get_device_name_for_udid "${udid}" "${mode}" )"
  if [ -z "${device_name}" ]; then
    echo "ERROR: no ${mode} found for udid [${udid}]"
    exit 1
  fi

  echo "Running on ${mode} [${device_name}] with udid [${udid}]..."
  echo "NOTE: press enter to terminate the app"
  echo
  sleep 1.5

  case "${mode}" in
    "sim")
      dotnet build "Apps/${app_name}" -t:run -f "${_tfm}" /p:_DeviceName=:v2:udid="${udid}"
      ;;
    "device")
      dotnet build "Apps/${app_name}" -t:run -f "${_tfm}" /p:_Device=:v2:udid="${udid}" /p:RuntimeIdentifier=ios-arm64
      ;;
  esac
}

main "${_app_name}" "${_mode}" "${_provided_udid}"

