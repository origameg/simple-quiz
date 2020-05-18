import { AnswerOption } from './answer-option';

export interface Question {
    
    id: string;
    question: string;
    answers: AnswerOption[];
    selectedAnswerId?: string;
    
}