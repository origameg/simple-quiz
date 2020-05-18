import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Quiz } from './quiz';
import { Score } from './score';
import { QuestionResponse } from './question-response';

@Injectable({
  providedIn: 'root'
})
export class ScoreService {

  private scoringUrl = 'api/v1/scoring';
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  constructor(
    private http: HttpClient) { }

  calculateScore(quiz: Quiz): Observable<Score> {

    let responses: QuestionResponse[] = [];
    quiz.questions.forEach(answeredQuestion => {
      responses.push({
        question: answeredQuestion.id, 
        answer: answeredQuestion.selectedAnswerId
      });
    });

    return this.http.post<Score>(this.scoringUrl, {responses: responses}, this.httpOptions).pipe(
      tap((score: Score) => { quiz.score = score; })
    );
  } 
}
