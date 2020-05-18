import { Component, OnInit, Input } from '@angular/core';
import { Question } from '../question';
import { QuestionService } from '../question.service';
import { Quiz } from '../quiz';
import { ScoreService } from '../score.service';

@Component({
  selector: 'app-questions',
  templateUrl: './questions.component.html',
  styleUrls: ['./questions.component.css']
})
export class QuestionsComponent implements OnInit {

  @Input() quiz: Quiz;

  index = 0;
  questions: Question[];
  selectedQuestion: Question;

  constructor(
    private questionService: QuestionService, 
    private scoringService: ScoreService) { }

  ngOnInit(): void {
    this.getQuestions();
  }

  onSelect(question: Question): void  {
    this.selectedQuestion = question;
  }

  onNextQuestion(): void  {
    this.index++;
    this.selectedQuestion = this.questions[this.index];
  }

  onScore(): void {
    this.selectedQuestion = null;
    this.quiz.questions = this.questions;
    this.scoringService.calculateScore(this.quiz).subscribe(score => {});
  }
 
  getQuestions(): void {
    this.questionService.getQuestions().subscribe(questions => {
      this.questions = questions;
      if (questions.length > 0) {
        this.selectedQuestion = questions[0];
      }
    });
  }

}
