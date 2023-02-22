import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CompanyCuDialogComponent } from './company-cu-dialog.component';

describe('CompanyCuDialogComponent', () => {
  let component: CompanyCuDialogComponent;
  let fixture: ComponentFixture<CompanyCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CompanyCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CompanyCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
