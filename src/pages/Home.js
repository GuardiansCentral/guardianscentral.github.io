import '../App.scss'
import TimerCard from '../components/TimerCard';
import Accordian from '../components/Accordian'
import TwitterTabs from '../components/TwitterTabs';
import './Home.scss'

const Home = (props) => {

    return(
        <div className="home">
            <div className='d-inline-flex flex-column justify-content-center w-100 gc-main-container'>
                <TimerCard/>
                <Accordian/>
                <TwitterTabs/>
                <h1>{props.serverMessage}</h1>
            </div>
        </div>
    );

}
export default Home;