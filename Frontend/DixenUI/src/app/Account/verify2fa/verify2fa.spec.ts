import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Verify2fa } from './verify2fa';

describe('Verify2fa', () => {
  let component: Verify2fa;
  let fixture: ComponentFixture<Verify2fa>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Verify2fa]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Verify2fa);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
