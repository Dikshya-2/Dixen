import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestRegistration } from './test-registration';

describe('TestRegistration', () => {
  let component: TestRegistration;
  let fixture: ComponentFixture<TestRegistration>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestRegistration]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestRegistration);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
