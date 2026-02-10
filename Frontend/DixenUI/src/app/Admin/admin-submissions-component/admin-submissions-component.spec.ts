import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminSubmissionsComponent } from './admin-submissions-component';

describe('AdminSubmissionsComponent', () => {
  let component: AdminSubmissionsComponent;
  let fixture: ComponentFixture<AdminSubmissionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminSubmissionsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminSubmissionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
