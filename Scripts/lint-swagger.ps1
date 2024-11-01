$Path = "$( $PSScriptRoot )/../WorkflowApi/.swagger"

docker run --rm --volume "$( $Path ):/work:ro" dshanley/vacuum lint -d swagger.yaml