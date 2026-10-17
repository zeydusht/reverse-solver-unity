#!/usr/bin/env bash
# Runs Unity in batchmode with the full log in Logs/batch/<name>.log and prints only
# the summary: test counts and failing tests, compile errors, exceptions, and the
# lines the method itself tags with [tag]. Unity must be closed (project lock).
#
#   Tools/unity_batch.sh tests                              all EditMode tests
#   Tools/unity_batch.sh tests "ReverseSolver.Tests.FunSim" one fixture/namespace (-testFilter)
#   Tools/unity_batch.sh method <Namespace.Class.Method> [extra Unity args...]
#
# Exit code: 0 when the run and every test passed, 1 otherwise.
set -u
UNITY="${UNITY:-/c/Program Files/Unity/Hub/Editor/6000.6.4f1/Editor/Unity.exe}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
OUT="$ROOT/Logs/batch"
mkdir -p "$OUT"
stamp=$(date +%Y%m%d-%H%M%S)
mode="${1:-}"; shift || true

summary_log() {  # $1 = log file
  local log="$1"
  grep -E "error CS[0-9]+" "$log" | sort -u | head -20
  grep -E "^[A-Za-z.]*Exception|Aborting batchmode|Scripts have compiler errors" "$log" | sort -u | head -10
  grep -E "^\[[a-z0-9-]+\]" "$log" | tail -40
}

case "$mode" in
  tests)
    log="$OUT/tests-$stamp.log"; xml="$OUT/tests-$stamp.xml"
    filter=(); [ $# -gt 0 ] && filter=(-testFilter "$1")
    start=$(date +%s)
    "$UNITY" -batchmode -nographics -projectPath "$ROOT" -runTests -testPlatform EditMode \
      -testResults "$xml" "${filter[@]}" -logFile "$log" >/dev/null 2>&1
    code=$?
    echo "tests: $(( $(date +%s) - start )) s, Unity exit $code, log $log"
    if [ -f "$xml" ]; then
      head -c 4000 "$xml" | grep -o '<test-run[^>]*>' | grep -oE '(total|passed|failed|skipped)="[0-9]+"' | tr '\n' ' '; echo
      grep -oE '<test-case [^>]*result="Failed"[^>]*>' "$xml" | grep -oE 'fullname="[^"]*"' | head -30
      grep -q 'result="Failed"' "$xml" && code=1
    else
      echo "no results xml"; summary_log "$log"; code=1
    fi
    exit $code ;;
  method)
    m="$1"; shift
    log="$OUT/$(echo "$m" | awk -F. '{print $(NF-1)"."$NF}')-$stamp.log"
    start=$(date +%s)
    "$UNITY" -batchmode -nographics -quit -projectPath "$ROOT" -executeMethod "$m" "$@" \
      -logFile "$log" >/dev/null 2>&1
    code=$?
    echo "$m: $(( $(date +%s) - start )) s, Unity exit $code, log $log"
    summary_log "$log"
    [ $code -eq 0 ] || exit 1 ;;
  *)
    sed -n '2,10p' "$0"; exit 2 ;;
esac
