import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboFinnishLineImportComponent } from './labo-finnish-line-import.component';

describe('LaboFinnishLineImportComponent', () => {
  let component: LaboFinnishLineImportComponent;
  let fixture: ComponentFixture<LaboFinnishLineImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboFinnishLineImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboFinnishLineImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
