//MAKE SURE TO CHANGE ENDPOINT BACK TO PROD BEFORE COMMITTING
import Carousel from 'react-bootstrap/Carousel';
import './WeeklyRotators.scss'
import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Button from 'react-bootstrap/Button';
import Card from 'react-bootstrap/Card';

const WeeklyRotators = () => {

    const [isSmallScreen, setIsSmallScreen] = useState(false);

    useEffect(() => {
      const handleResize = () => {
        setIsSmallScreen(window.innerWidth < 992);
      };
  
      // Initial check on mount
      handleResize();
  
      // Add event listener for window resize
      window.addEventListener('resize', handleResize);
  
      // Clean up the event listener on component unmount
      return () => {
        window.removeEventListener('resize', handleResize);
      };
    }, []);

    const [isMobile, setIsMobile] = useState(false);

    useEffect(() => {
      const handleResize = () => {
        setIsMobile(window.innerWidth < 600);
      };
  
      // Initial check on mount
      handleResize();
  
      // Add event listener for window resize
      window.addEventListener('resize', handleResize);
  
      // Clean up the event listener on component unmount
      return () => {
        window.removeEventListener('resize', handleResize);
      };
    }, []);

    //Raid info states
    const [raidWeaponDict, setRaidWeaponDict] = useState({});
    const [raidTitanArmorDict, setRaidTitanArmorDict] = useState({});
    const [raidHunterArmorDict, setRaidHunterArmorDict] = useState({});
    const [raidWarlockArmorDict, setRaidWarlockArmorDict] = useState({});
    const [raidCosmeticDict, setRaidCosmeticDict] = useState({});
    const [raidName, setRaidName] = useState([]);
    const [raidImage, setRaidImage] = useState([]);

    //Exotic Quest info states
    const [exoticQuestWeaponDict, setExoticQuestWeaponDict] = useState({});
    const [exoticQuestTitanArmorDict, setExoticQuestTitanArmorDict] = useState({});
    const [exoticQuestHunterArmorDict, setExoticQuestHunterArmorDict] = useState({});
    const [exoticQuestWarlockArmorDict, setExoticQuestWarlockArmorDict] = useState({});
    const [exoticQuestCatalystDict, setExoticQuestCatalystDict] = useState({});
    const [exoticQuestName, setExoticQuestName] = useState([]);
    const [exoticQuestImage, setExoticQuestImage] = useState([]);

    //Dungeon info states
    const [dungeonWeaponDict, setDungeonWeaponDict] = useState({});
    const [dungeonTitanArmorDict, setDungeonTitanArmorDict] = useState({});
    const [dungeonHunterArmorDict, setDungeonHunterArmorDict] = useState({});
    const [dungeonWarlockArmorDict, setDungeonWarlockArmorDict] = useState({});
    const [dungeonCosmeticDict, setDungeonCosmeticDict] = useState({});
    const [dungeonName, setDungeonName] = useState([]);
    const [dungeonImage, setDungeonImage] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await axios.get('https://guardianscentral.gg/weeklyrotators');
                //const response = await axios.get('http://localhost:8000/weeklyrotators');
                const data = response.data.getWeeklyRotators;
                //console.log(data);
                // Parse the JSON data and set the Dicts
                data.forEach(item => {
                    const jsonData = JSON.parse(item.Json);
                    const activityType = jsonData.activityType;
                    const weapons = jsonData.weapons;
                    const titanArmor = jsonData.titanArmor;
                    const hunterArmor = jsonData.hunterArmor;
                    const warlockArmor = jsonData.warlockArmor;
                    const cosmetics = jsonData.cosmetics;
                    const activityName = jsonData.activityName;
                    const activityImage = jsonData.pcgrImage;
                    const catalyst = jsonData.catalysts;

                    switch (activityType) {
                        case 'Raid':
                            if(weapons != null){
                                const weaponDict = {};
                                weapons.forEach(weaponObj => {
                                    const weaponName = Object.keys(weaponObj)[0];
                                    const weaponData = weaponObj[weaponName];
                                    weaponDict[weaponName] = weaponData;
                                });
                                setRaidWeaponDict(weaponDict);
                            };
                            if(titanArmor != null){
                                const titanArmorDict = {};
                                titanArmor.forEach(titanArmorObj => {
                                    const titanArmorName = Object.keys(titanArmorObj)[0];
                                    const titanArmorData = titanArmorObj[titanArmorName];
                                    titanArmorDict[titanArmorName] = titanArmorData;
                                });
                                setRaidTitanArmorDict(titanArmorDict);
                            };
                            if(hunterArmor != null){
                                const hunterArmorDict = {};
                                hunterArmor.forEach(hunterArmorObj => {
                                    const hunterArmorName = Object.keys(hunterArmorObj)[0];
                                    const hunterArmorData = hunterArmorObj[hunterArmorName];
                                    hunterArmorDict[hunterArmorName] = hunterArmorData;
                                });
                                setRaidHunterArmorDict(hunterArmorDict);
                            };
                            if(warlockArmor != null){
                                const warlockArmorDict = {};
                                warlockArmor.forEach(warlockArmorObj => {
                                    const warlockArmorName = Object.keys(warlockArmorObj)[0];
                                    const warlockArmorData = warlockArmorObj[warlockArmorName];
                                    warlockArmorDict[warlockArmorName] = warlockArmorData;
                                });
                                setRaidWarlockArmorDict(warlockArmorDict);
                            };
                            if(cosmetics != null){
                                const cosmeticDict = {};
                                cosmetics.forEach(cosmeticObj => {
                                    const cosmeticName = Object.keys(cosmeticObj)[0];
                                    const cosmeticData = cosmeticObj[cosmeticName];
                                    cosmeticDict[cosmeticName] = cosmeticData;
                                });
                                setRaidCosmeticDict(cosmeticDict);
                            };
                            if(activityName != null){
                                setRaidName(activityName);
                            }
                            if(activityImage != null){
                                setRaidImage(activityImage);
                            };
                            break;
                        case 'Dungeon':
                            if(weapons != null){
                                const weaponDict = {};
                                weapons.forEach(weaponObj => {
                                    const weaponName = Object.keys(weaponObj)[0];
                                    const weaponData = weaponObj[weaponName];
                                    weaponDict[weaponName] = weaponData;
                                });
                                setDungeonWeaponDict(weaponDict);
                            };
                            if(titanArmor != null){
                                const titanArmorDict = {};
                                titanArmor.forEach(titanArmorObj => {
                                    const titanArmorName = Object.keys(titanArmorObj)[0];
                                    const titanArmorData = titanArmorObj[titanArmorName];
                                    titanArmorDict[titanArmorName] = titanArmorData;
                                });
                                setDungeonTitanArmorDict(titanArmorDict);
                            };
                            if(hunterArmor != null){
                                const hunterArmorDict = {};
                                hunterArmor.forEach(hunterArmorObj => {
                                    const hunterArmorName = Object.keys(hunterArmorObj)[0];
                                    const hunterArmorData = hunterArmorObj[hunterArmorName];
                                    hunterArmorDict[hunterArmorName] = hunterArmorData;
                                });
                                setDungeonHunterArmorDict(hunterArmorDict);
                            };
                            if(warlockArmor != null){    
                                const warlockArmorDict = {};
                                warlockArmor.forEach(warlockArmorObj => {
                                    const warlockArmorName = Object.keys(warlockArmorObj)[0];
                                    const warlockArmorData = warlockArmorObj[warlockArmorName];
                                    warlockArmorDict[warlockArmorName] = warlockArmorData;
                                });
                                setDungeonWarlockArmorDict(warlockArmorDict);
                            };
                            if(cosmetics != null){
                                const cosmeticDict = {};
                                cosmetics.forEach(cosmeticObj => {
                                    const cosmeticName = Object.keys(cosmeticObj)[0];
                                    const cosmeticData = cosmeticObj[cosmeticName];
                                    cosmeticDict[cosmeticName] = cosmeticData;
                                });
                                setDungeonCosmeticDict(cosmeticDict);
                            };
                            if(activityName != null){
                                setDungeonName(activityName);
                            };
                            if(activityImage != null){
                                setDungeonImage(activityImage);
                            };
                            break;
                        case 'Story':
                            if(weapons != null){
                                const weaponDict = {};
                                weapons.forEach(weaponObj => {
                                    const weaponName = Object.keys(weaponObj)[0];
                                    const weaponData = weaponObj[weaponName];
                                    weaponDict[weaponName] = weaponData;
                                });
                                setExoticQuestWeaponDict(weaponDict);
                            };
                            if(titanArmor != null){
                                const titanArmorDict = {};
                                titanArmor.forEach(titanArmorObj => {
                                    const titanArmorName = Object.keys(titanArmorObj)[0];
                                    const titanArmorData = titanArmorObj[titanArmorName];
                                    titanArmorDict[titanArmorName] = titanArmorData;
                                });
                                setExoticQuestTitanArmorDict(titanArmorDict);
                            };

                            if(hunterArmor != null){
                                const hunterArmorDict = {};
                                hunterArmor.forEach(hunterArmorObj => {
                                    const hunterArmorName = Object.keys(hunterArmorObj)[0];
                                    const hunterArmorData = hunterArmorObj[hunterArmorName];
                                    hunterArmorDict[hunterArmorName] = hunterArmorData;
                                });
                                setExoticQuestHunterArmorDict(hunterArmorDict);
                            };
                            if(warlockArmor != null){
                                const warlockArmorDict = {};
                                warlockArmor.forEach(warlockArmorObj => {
                                    const warlockArmorName = Object.keys(warlockArmorObj)[0];
                                    const warlockArmorData = warlockArmorObj[warlockArmorName];
                                    warlockArmorDict[warlockArmorName] = warlockArmorData;
                                });
                                setExoticQuestWarlockArmorDict(warlockArmorDict);
                            };
                            if(catalyst != null){
                                const catalystDict = {};
                                catalyst.forEach(catalystObj => {
                                    const catalystName = Object.keys(catalystObj)[0];
                                    const catalystData = catalystObj[catalystName];
                                    catalystDict[catalystName] = catalystData;
                                });
                                setExoticQuestCatalystDict(catalystDict);
                            }
                            if(activityName != null){
                                setExoticQuestName(activityName);
                            };
                            if(activityImage != null){
                                setExoticQuestImage(activityImage);
                            };
                            break;
                        default:
                            break;
                    }

                });
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };
    
        fetchData();
    }, []);

    // Modify the state to track the clicked item
    const [clickedItem, setClickedItem] = useState(null);

    // Modify the click event handler
    const handleClick = (item) => {
        // Toggle the display of the clicked item
        setClickedItem(clickedItem === item ? null : item);
    };
    const handleClose = () => {
        setClickedItem(null);
    };
    const renderItems = (dict) => {
        const shouldAddClass = Object.keys(dict).length >= 2;
        return Object.entries(dict).map(([itemName, itemData]) => (
            <img 
                key={itemName} 
                className={`img-fluid ${shouldAddClass ? 'wr-img-icon' : ''}`} 
                src={"https://www.bungie.net" + itemData.icon} 
                alt={itemName} 
                onClick={() => handleClick(itemData)} 
            />
        ));
    }
    const tierTypeColors = {
        "Legendary": "purple",
        "Exotic": "#ceae33",
        "Default": "black"
    };
    return(
        <>
          <Carousel>
            <Carousel.Item interval={1000000}>
            <div className='wr-overlay-container'>
                    <img className="carousel-img-transform" src={"https://www.bungie.net" + raidImage} alt={raidName + " Background"} />
                    <div className='wr-overlay'></div>
                </div>
                <Carousel.Caption className='top-0 wr-overlay-text fw-bold overflow-auto'>
                    <h2 className='fw-bold'>Weekly Raid</h2>
                    <h3 className='fw-bold'>{raidName}</h3>
                    <div className={`info-container ${isSmallScreen ? 'flex-column' : 'd-inline-flex'}`}>
                        <div className='wr-img-column-container px-1'>
                            <h5 className='fw-bold'>Weapons</h5>
                            {renderItems(raidWeaponDict)}
                        </div>
                        <div className='wr-img-column-container px-1'>
                            <h5 className='fw-bold'>Armor</h5>
                            <div className='Titan'>
                                {renderItems(raidTitanArmorDict)}
                            </div>
                            <div className='Hunter'>
                                {renderItems(raidHunterArmorDict)}
                            </div>
                            <div className='Warlock'>
                                {renderItems(raidWarlockArmorDict)}
                            </div>
                        </div>
                        <div className='wr-img-column-container px-1'>
                            <h5 className='fw-bold'>Cosmetics</h5>
                             {renderItems(raidCosmeticDict)}
                        </div>
                    </div>
                </Carousel.Caption>
            </Carousel.Item>
            <Carousel.Item interval={1000000}>
            <div className='wr-overlay-container'>
                    <img className="carousel-img-transform" src={"https://www.bungie.net" + dungeonImage} alt={raidName + " Background"} />
                    <div className='wr-overlay'></div>
                </div>
                <Carousel.Caption className='top-0 wr-overlay-text fw-bold overflow-auto'>
                    <h2 className='fw-bold'>Weekly Dungeon</h2>
                    <h3 className='fw-bold'>{dungeonName}</h3>
                    <div className={`info-container ${isSmallScreen ? 'flex-column' : 'd-inline-flex'}`}>
                        <div className='wr-img-column-container px-1'>
                            <h5 className='fw-bold'>Weapons</h5>
                            {renderItems(dungeonWeaponDict)}
                        </div>
                        <div className='wr-img-column-container px-1'>
                            <h5 className='fw-bold'>Armor</h5>
                            <div className='Titan'>
                                {renderItems(dungeonTitanArmorDict)}
                            </div>
                            <div className='Hunter'>
                                {renderItems(dungeonHunterArmorDict)}
                            </div>
                            <div className='Warlock'>
                                {renderItems(dungeonWarlockArmorDict)}
                            </div>
                        </div>
                        {Object.keys(dungeonCosmeticDict).length > 0 && (
                            <div className='wr-img-column-container px-1'>
                                <h5 className='fw-bold'>Cosmetics</h5>
                                {renderItems(dungeonCosmeticDict)}
                            </div> 
                        )}
                    </div>
                </Carousel.Caption>
            </Carousel.Item>
            <Carousel.Item interval={1000000}>
            <div className='wr-overlay-container'>
                    <img className="carousel-img-transform" src={"https://www.bungie.net" + exoticQuestImage} alt={raidName + " Background"} />
                    <div className='wr-overlay'></div>
                </div>
                <Carousel.Caption className='top-0 wr-overlay-text fw-bold overflow-auto'>
                    <h2 className='fw-bold'>Weekly Exotic Quest</h2>
                    <h3 className='fw-bold'>{exoticQuestName}</h3>
                    <p>You get 3 guaranteed red border drops per week. <br></br>One for completing legend, one for master and one when the pinnacle reward is completed</p>
                    <div className={`info-container ${isSmallScreen ? 'flex-column' : 'd-inline-flex'}`}>
                  
                        <div className='wr-img-column-container px-1'>
                            <h5 className='fw-bold'>Weapons</h5>
                            {renderItems(exoticQuestWeaponDict)}
                        </div>
                        {(Object.keys(exoticQuestTitanArmorDict).length > 0 || Object.keys(exoticQuestHunterArmorDict).length > 0 || Object.keys(exoticQuestWarlockArmorDict).length > 0) && (
                        <div className='wr-img-column-container px-1'>
                            <h5 className='fw-bold'>Armor</h5>
                            <div className='Titan'>
                                {renderItems(exoticQuestTitanArmorDict)}
                            </div>
                            <div className='Hunter'>
                                {renderItems(exoticQuestHunterArmorDict)}
                            </div>
                            <div className='Warlock'>
                                {renderItems(exoticQuestWarlockArmorDict)}
                            </div>
                        </div>
                        )}
                        {Object.keys(exoticQuestCatalystDict).length > 0 && (
                        <div className='wr-img-column-container px-1'>
                            <h5 className='fw-bold'>Catalyst</h5>
                             {renderItems(exoticQuestCatalystDict)}
                        </div>
                        )}
                    </div>
                </Carousel.Caption>
            </Carousel.Item>
            {/* This is for loading content on anything but mobile*/}
            {clickedItem && !isMobile  &&
                (
                    <div className="clicked-item">
                        <Card className='weapon-info'>
                        <Button variant="danger" className="close-button" onClick={handleClose}>X</Button>
                        {clickedItem.screenshot && (
                            <Card.Img variant="top" src={"https://www.bungie.net" + clickedItem.screenshot} />
                        )}
                        {clickedItem.secondaryIcon && (
                            <Card.Img variant="top" src={"https://www.bungie.net" + clickedItem.secondaryIcon} />
                        )}
                        <div className='d-flex flex-row text-light' style={{background: tierTypeColors[clickedItem?.tierTypeName] || tierTypeColors["Default"]}}>
                            <div className='d-flex flex-column flex-grow-1 align-items-start p-2'>
                                <h2>{clickedItem.name}</h2>
                                <h5>{clickedItem.typeAndTierDisplayName}</h5>
                            </div>
                            {clickedItem.damageTypeIcon && (
                            <div className='p-2'>
                                <img className='damage-type-icon' src={"https://www.bungie.net" + clickedItem['damageTypeIcon']} alt={clickedItem.damageTypeName}/>
                            </div>
                            )}
                        </div>
                        {(clickedItem.damageTypeIcon || clickedItem.rpmStat || clickedItem.frameName || clickedItem.frameDescription || clickedItem.description) && (
                        <div className='d-flex flex-row text-light justify-content-between' style={{background:'#6c757d'}}>
                            {clickedItem.frameIcon && (
                            <div className='align-self-center p-2'>
                                <img className='frame-icon' src={"https://www.bungie.net" + clickedItem['frameIcon']} alt={clickedItem.frameName}></img>
                            </div>
                            )}
                            {clickedItem.frameName ? 
                                (
                                <div className='align-self-center p-2'>
                                    <h5>{clickedItem['frameName']}</h5>
                                    <p>{clickedItem['frameDescription']}</p>  
                                </div>
                                ) : (
                                <div className='align-self-center p-2'>
                                    <p>{clickedItem['description']}</p>  
                                </div>
                                )
                            }
                            {clickedItem.rpmStat && (
                            <div className='d-flex flex-column align-self-center p-3'>
                                <span>RPM </span>
                                <span>{clickedItem['rpmStat']}</span>
                            </div>
                            )}
                        </div>
                        )}
                        </Card>
                    </div>
                )
            }
          </Carousel>
            {/* This is for loading content on mobile */}
            {clickedItem && isMobile && 
                (
                    <div className="clicked-item">
                        <Card className='weapon-info mt-5'>
                        <Button variant="danger" className="close-button" onClick={handleClose}>X</Button>
                        {clickedItem.screenshot && (
                            <Card.Img variant="top" src={"https://www.bungie.net" + clickedItem.screenshot} />
                        )}
                        {clickedItem.secondaryIcon && (
                            <Card.Img variant="top" src={"https://www.bungie.net" + clickedItem.secondaryIcon} />
                        )}
                            <div className='d-flex flex-row text-light' style={{background: tierTypeColors[clickedItem?.tierTypeName] || tierTypeColors["Default"]}}>
                                    <div className='d-flex flex-column flex-grow-1 align-items-start p-2'>
                                        <h2>{clickedItem.name}</h2>
                                        <h5>{clickedItem.typeAndTierDisplayName}</h5>
                                    </div>
                                    {clickedItem.damageTypeIcon && (
                                    <div className='p-2'>
                                        <img className='damage-type-icon' src={"https://www.bungie.net" + clickedItem['damageTypeIcon']} alt={clickedItem.damageTypeName}/>
                                    </div>
                                    )}
                                </div>
                                {(clickedItem.damageTypeIcon || clickedItem.rpmStat || clickedItem.frameName || clickedItem.frameDescription || clickedItem.description) && (
                                <div className='d-flex flex-row text-light justify-content-between' style={{background:'#6c757d'}}>
                                    {clickedItem.damageTypeIcon && (
                                    <div className='align-self-center p-2'>
                                        <img className='frame-icon' src={"https://www.bungie.net" + clickedItem['frameIcon']} alt={clickedItem.damageTypeName}></img>
                                    </div>
                                    )}
                                    {clickedItem.frameName ? 
                                        (
                                        <div className='align-self-center p-2'>
                                            <h5>{clickedItem['frameName']}</h5>
                                            <p>{clickedItem['frameDescription']}</p>  
                                        </div>
                                        ) : (
                                        <div className='align-self-center p-2'>
                                            <p>{clickedItem['description']}</p>  
                                        </div>
                                        )
                                    }
                                    {clickedItem.rpmStat && (
                                    <div className='d-flex flex-column align-self-center p-2'>
                                        <span>RPM </span>
                                        <span>{clickedItem['rpmStat']}</span>
                                    </div>
                                    )}
                                    
                                </div>
                                )}
                        </Card>
                    </div>
                )
            }
        </>
          
    );
}

export default WeeklyRotators