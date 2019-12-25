import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedDemoDataDialogComponent } from './shared-demo-data-dialog.component';

describe('SharedDemoDataDialogComponent', () => {
  let component: SharedDemoDataDialogComponent;
  let fixture: ComponentFixture<SharedDemoDataDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SharedDemoDataDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SharedDemoDataDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
