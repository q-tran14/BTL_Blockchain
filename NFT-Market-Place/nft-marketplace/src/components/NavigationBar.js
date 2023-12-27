import {
    Link
} from "react-router-dom";
import {Navbar, Nav, Button, Container } from 'react-bootstrap'
import icon from '../../src/images/Marketplace-icon.png';
import '../css/Navbar.css';
<script></script>

const Navigation = ({ web3Handler, account }) => {

    return (
        <Navbar expand="lg" className="bg-img">
            <Container className="font-face-changa-one">
                <img src={icon} width="40" height="40" className="" alt="" />
                <Navbar.Brand href="#action/3.1" as={Link} to="/Marketplace">Axie Marketplace</Navbar.Brand>

                <Navbar.Collapse id="basic-navbar-nav">
                    <Nav className="me-auto center-navbar">
                        <Nav.Link href="#action/3.1" as={Link} to="/Marketplace">Marketplace</Nav.Link>
                        <Nav.Link href="#action/3.1" as={Link} to="/Collection">Collection</Nav.Link>
                        {account ? (
                                <Nav.Link
                                    href={`https://etherscan.io/address/${account}`}
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    className="button nav-button btn-sm mx-4">
                                    {/* <Button variant="success"> */}
                                    <Button id="loginBtn" variant="outline-light">
                                        {account.slice(0, 5) + '...' + account.slice(38, 42)}
                                    </Button>

                                </Nav.Link>
                            ) : (
                                <Button id="loginBtn" onClick={web3Handler} variant="outline-light">Connect Wallet</Button>
                            )}
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    )
}
export default Navigation;