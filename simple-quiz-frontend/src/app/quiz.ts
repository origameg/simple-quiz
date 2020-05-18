import { Question } from './question';
import { Score } from './score';

export interface Quiz {
    
    questions?: Question[];
    score?: Score;
}