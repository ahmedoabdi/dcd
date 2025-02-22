import { Button } from "@equinor/eds-core-react"
import styled from "styled-components"

export const AssetHeader = styled.div`
    margin-bottom: 2rem;
    display: flex;

    > *:first-child {
        margin-right: 2rem;
    }
`

export const AssetViewDiv = styled.div`
    margin: 2rem;
    display: flex;
    flex-direction: column;
`

export const Wrapper = styled.div`
    display: flex;
    flex-direction: row;
`

export const WrapperTablePeriod = styled.div`
    display: flex;
    flex-direction: row;
    margin-left: 70rem;
    margin-bottom: -1rem;
    align-content: right;
`

export const WrapperInherited = styled.div`
    display: flex;
    flex-direction: row;
    > *:not(:last-child) {
        font-weight: bold;
    }
`

export const WrapperRow = styled.div`
    display: flex;
    flex-direction: row;
    align-content: center;
    margin-bottom: 1rem;
    `

export const WrapperColumn = styled.div`
    display: flex;
    flex-direction: column;
`

export const WrapperColumnTablePeriod = styled.div`
    display: flex;
    flex-direction: column;
    margin-right: 1rem;
    margin-left: 1rem;
    margin-top: -1rem;
`

export const SaveButton = styled(Button)`
    margin-left: 3rem;
    width: 5rem;
    &:disabled {
        margin-left: 3rem;
    }
`

export const Dg4Field = styled.div`
    margin-right: 1rem;
    margin-left: 1rem;
    margin-bottom: 2rem;
    width: 10rem;
    display: flex;
`

export const ImportButton = styled(Button)`
    width: 10rem;
    &:disabled {
        margin-left: 2rem;
    }
`

export const DeleteButton = styled(Button)`
    margin-left: 2rem;
    width: 10rem;
    &:disabled {
        margin-left: 2rem;
    }
`
