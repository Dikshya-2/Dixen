import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecommendedEvent } from './recommended-event';

describe('RecommendedEvent', () => {
  let component: RecommendedEvent;
  let fixture: ComponentFixture<RecommendedEvent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecommendedEvent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecommendedEvent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
