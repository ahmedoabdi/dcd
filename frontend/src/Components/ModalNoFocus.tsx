import { FunctionComponent } from "react"
import styled from "styled-components"
import { Typography } from "@equinor/eds-core-react"

const ModalDiv = styled.div`
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    padding: 30px;
    z-index: 1000;
    background-color: white;
    border: 2px solid gray;
`

type Props = {
    title: string;
    isOpen: boolean;
}

export const ModalNoFocus: FunctionComponent<Props> = ({
    isOpen, title, children,
}) => {
    if (!isOpen) return null
    return (
        <>
            <div style={{
                position: "fixed",
                top: 0,
                left: 0,
                right: 0,
                bottom: 0,
                backgroundColor: "rgba(0,0,0, .7)",
                zIndex: 1000,
            }}
            />
            <ModalDiv>
                {title && <Typography variant="h1">{title}</Typography>}
                <div>{children}</div>
            </ModalDiv>
        </>
    )
}
