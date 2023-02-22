import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedErrorDialogComponent } from './shared-error-dialog.component';

describe('SharedErrorDialogComponent', () => {
  let component: SharedErrorDialogComponent;
  let fixture: ComponentFixture<SharedErrorDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SharedErrorDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SharedErrorDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
