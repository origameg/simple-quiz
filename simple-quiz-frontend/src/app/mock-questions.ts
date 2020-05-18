import { Question } from './question';

export const QUESTIONS: Question[] = [
    { id: 'q-0', question: 'Which Norwegian polar explorer built the first base on the Antarctic continent?', answers: [
        {id: '0-0', text:'Roald Amundsen'},
        {id: '0-1', text:'Otto Sverdrup'},
        {id: '0-2', text:'Carsten Borchgrevink'},
        {id: '0-3', text:'Fridtjof Nansen'}  
    ] },
    { id: 'q-1', question: 'What animal appears on the flag of the Turks and Caicos Islands?', answers: [
        {id: '1-0', text:'turtle'},
        {id: '1-1', text:'lion'},
        {id: '1-2', text:'lobster'},
        {id: '1-3', text:'albatross'}
    ] },
    { id: 'q-2', question: 'Which country was not one of the original 12 signatories of the Antarctic Treaty?', answers: [
        {id: '2-0', text:'Germany'},
        {id: '2-1', text:'Chile'},
        {id: '2-2', text:'Belgium'},
        {id: '2-3', text:'Japan'}
    ] },
    { id: 'q-3', question: 'What is the call sign of Mike Metcalf, the lead instructor in Top Gun?', answers: [
        {id: '3-0', text:'Jester'},
        {id: '3-1', text:'Viper'},
        {id: '3-2', text:'Wolfman'},
        {id: '3-3', text:'Slider'}
    ] }
];