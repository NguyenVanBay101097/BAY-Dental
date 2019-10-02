import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RoutingLineCuDialogComponent } from './routing-line-cu-dialog.component';

describe('RoutingLineCuDialogComponent', () => {
  let component: RoutingLineCuDialogComponent;
  let fixture: ComponentFixture<RoutingLineCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RoutingLineCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RoutingLineCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
