// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721URIStorage.sol";
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Pausable.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

contract AxICToken is ERC721, ERC721Enumerable, ERC721URIStorage, ERC721Pausable, Ownable {
    uint256 private _nextTokenId;

    

    constructor(address initialOwner)
        ERC721("AxICToken", "AxI")
        Ownable(initialOwner)
    {}

    function safeMint(address to, string memory uri) public payable{
        uint256 tokenId = _nextTokenId++;
        _safeMint(to, tokenId);
        _setTokenURI(tokenId, uri);
    }

    function getAllToken(address owner) external view returns (uint256[] memory){
        uint256 total = balanceOf(owner);
        uint256[] memory ids = new uint256[](total);
        for (uint256 i = 0; i < total; i++){
            ids[i] = (IERC721Enumerable(this).tokenOfOwnerByIndex(owner, i));
        }
        return ids;
    }
    
    function pause() public onlyOwner {
        _pause();
    }

    function unpause() public onlyOwner {
        _unpause();
    }
    // The following functions are overrides required by Solidity.

    function _update(address to, uint256 tokenId, address auth) internal override(ERC721, ERC721Enumerable, ERC721Pausable) returns (address)
    {
        return super._update(to, tokenId, auth);
    }

    function _increaseBalance(address account, uint128 value) internal override(ERC721, ERC721Enumerable)
    {
        super._increaseBalance(account, value);
    }

    function tokenURI(uint256 tokenId) public view override(ERC721, ERC721URIStorage) returns (string memory)
    {
        return super.tokenURI(tokenId);
    }

    function supportsInterface(bytes4 interfaceId) public view override(ERC721, ERC721Enumerable, ERC721URIStorage) returns (bool)
    {
        return super.supportsInterface(interfaceId);
    }
}