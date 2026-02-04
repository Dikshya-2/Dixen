import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConformEmail } from './conform-email';

describe('ConformEmail', () => {
  let component: ConformEmail;
  let fixture: ComponentFixture<ConformEmail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConformEmail]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConformEmail);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
