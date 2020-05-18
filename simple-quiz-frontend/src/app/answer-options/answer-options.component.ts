import { Component, OnInit, Input } from '@angular/core';
import { Question } from '../question';
import { AnswerOption } from '../answer-option';

@Component({
  selector: 'app-answer-options',
  templateUrl: './answer-options.component.html',
  styleUrls: ['./answer-options.component.css']
})

export class AnswerOptionsComponent implements OnInit {

  @Input() question: Question;

  constructor() { }

  ngOnInit(): void {
  }

  onSelect(answer: AnswerOption): void  {
    this.question.selectedAnswerId = answer.id;
  }

}
