Imports System.IO
Imports System.Threading
Imports System.Math
Imports System.Runtime.InteropServices
Public Class Form_Main
#Const TargetOS = "x64"

#If TargetOS = "linux" Then
    <DllImport("./libDIVA.so")> Function divapro(ByVal vbtreeline() As Char, ByRef genno As Integer) As Integer
    End Function
    <DllImport("./libconsence.so")> Function ctree(ByVal vbOG As Int16, ByVal path As String, ByRef pros As Integer) As Integer
    End Function
#ElseIf TargetOS = "x64" Or TargetOS = "macos" Then
    <DllImport("BAYAREA.dll")> Public Shared Function runbayarea(ByVal pam1 As String, ByRef genno As Integer) As Integer
    End Function
    <DllImport("BAYAREA.dll")> Public Shared Function fnBAYAREADLL(ByVal pam1 As String) As Integer
    End Function
    <DllImport("CONSDLL.dll")> Public Shared Function ctree(ByVal vbOG As Int16, ByVal path As String, ByRef pros As Integer) As Integer
    End Function
    <DllImport("LAGDLL.dll")> Public Shared Function runlag(ByVal path As String, ByVal filename As String, ByRef genno As Integer) As Integer
    End Function
    <DllImport("DIVADLL.dll")> Public Shared Function divapro(ByVal vbtreeline() As Char, ByRef genno As Integer) As Integer
    End Function
#End If
    Dim Begin_Show As Boolean = False
    Delegate Sub SetText(ByVal text As String)
    Dim printrecs As Boolean = True
    Dim P_mode As String = "optimal distributions"
    Dim dec_mode As Integer = 0
    Dim wait_m_s As Integer = 10
    Dim optimize As String = "optimize"
    Dim optimize_f As String = "optimize"
    Dim Halt As Boolean = False
    Dim Halt_Msg As String = ""
    Dim DistributionLine As String = ""
    Dim outgroup As Boolean = True
    Dim B_Tree_File As String = ""
    Dim P_Tree_File As String = ""
    Dim tree_path As String
    Dim final_tree_file As String = ""
    Dim Write_Tree_Info As Boolean = False
    Public Taxon_Dataset As New DataSet
    Dim Node_Dataset As New DataSet
    Dim Tree_Num_B As Integer = 0
    Dim Tree_Num_P As Integer = 0
    Dim sdec_id As Integer = 0
    Dim sdec_count As Integer = 0
    Dim taxon_line As String
    Dim num_r1() As Single
    Dim num_r2() As Single
    Dim node(,) As String
    Dim clade_p(,) As Single
    Dim tree_char() As String
    Delegate Sub SetNum(ByVal text As Integer)
    Dim RT1 As SetText
    Dim RT2 As SetText
    Dim RT1_S As SetText
    Dim RT2_S As SetText
    Dim PV As SetNum
    Dim RTB As SetText
    Dim TB1 As SetText
    Dim TB2 As SetText
    Dim TB3 As SetText
    Dim TB4 As SetText
    Dim SetTI As SetText
    Dim Enable_BBM As Boolean = False
    Dim SBGB_count As Integer
    Dim SACE_count As Integer
    Private Sub TB3_SetText(ByVal str As String)
        TreeBox_P.Text = CInt(str)
    End Sub
    Private Sub CmdBox_SetText(ByVal str As String)
        CmdBox.Text = str
    End Sub
    Private Sub StatBox_SetText(ByVal str As String)
        statebox.Text = str
    End Sub
    Private Sub StatBox_AppendText(ByVal str As String)
        statebox.AppendText(str)
    End Sub
    Private Sub RTB_SetText(ByVal str As String)
        RandomTextBox.Text = CInt(str)
    End Sub
    Private Sub PV_SetText(ByVal str As Integer)
        ProgressBar1.Value = str
    End Sub
    Private Sub CmdBox_AppendText(ByVal str As String)
        CmdBox.AppendText(str)
    End Sub
    Private Sub TreeBox_SetText(ByVal str As String)
        TreeBox.Text = str
    End Sub
    Private Sub FinalTreeBox_SetText(ByVal str As String)
        FinalTreeBox.Text = str
    End Sub
    Private Sub BurninBox_SetText(ByVal str As String)
        BurninBox.Text = str
    End Sub
    Private Sub TreeInfo_SetText(ByVal str As String)
        TreeInfo.AppendText(str)
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Begin_Show = False
        Me.Visible = False
        Dim filePath As String = root_path + "Plug-ins\" + "setting.ini"
        SaveSettings(filePath, settings)
        Try
            If isDebug = False Then
                DeleteDir(root_path + "temp")
            End If
        Catch ex As Exception
        End Try
        End
    End Sub

   

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CmdBox_SetText("To maintain RASP and support users, it is important for us that you cite our paper:" + vbCrLf)
        CmdBox_AppendText("Yu Y, Blair C, He XJ. 2020. RASP 4: Ancestral State Reconstruction Tool for Multiple Genes and Characters. Molecular Biology and Evolution. 37(2):604-606")
        If TargetOS = "x64" Then
            ToolStripSeparator7.Visible = True
            InstallReinstallToolStripMenuItem.Visible = True
        Else
            ToolStripSeparator7.Visible = False
            InstallReinstallToolStripMenuItem.Visible = False
        End If
     
        DatingToolStripMenuItem.Enabled = isDebug
        DebugToolStripMenuItem.Visible = isDebug

        System.Threading.Thread.CurrentThread.CurrentCulture = ci
		Main_Timer.Enabled = True
        Me.Text = Me.Text + " " + Version + " build " + build + " " + TargetOS
        Me.MinimizeBox = enableMin
		RT1 = New SetText(AddressOf CmdBox_AppendText)
		RT2 = New SetText(AddressOf StatBox_AppendText)
		RT1_S = New SetText(AddressOf CmdBox_SetText)
		RT2_S = New SetText(AddressOf StatBox_SetText)
		TB1 = New SetText(AddressOf TreeBox_SetText)
		PV = New SetNum(AddressOf PV_SetText)
		RTB = New SetText(AddressOf RTB_SetText)
		TB2 = New SetText(AddressOf BurninBox_SetText)
		TB3 = New SetText(AddressOf TB3_SetText)
		TB4 = New SetText(AddressOf FinalTreeBox_SetText)
		SetTI = New SetText(AddressOf TreeInfo_SetText)
		format_path()
		OpenData()
	End Sub

	Public Sub Make_Optimize()
		If Me.InvokeRequired Then
			Me.Invoke(New MethodInvoker(AddressOf Make_Optimize))
		Else
			optimize = "optimize"
			If DIVAForm.CheckBox1.Checked Then
				optimize = optimize + " enex"
			End If
			If DIVAForm.CheckBox2.Checked Then
				optimize = optimize + " Printrecs"
			End If
			If DIVAForm.CheckBox4.Checked Then
				optimize = optimize + " Maxareas=" + DIVAForm.NumericUpDown2.Value.ToString
			End If
			If DIVAForm.CheckBox5.Checked Then
				optimize = optimize + " min_t=" + taxon_num.ToString + " rand=" + DIVAForm.NumericUpDown1.Value.ToString
			Else
				optimize = optimize + " min_t=65536 rand=" + DIVAForm.NumericUpDown1.Value.ToString
			End If
			If DIVAForm.CheckBox9.Checked Then
				optimize = optimize + " keep=" + DIVAForm.TextBox1.Text
			End If
			optimize_f = optimize
			If DIVAForm.CheckBox2.Checked Then
				optimize = optimize + " max_a=" + DIVAForm.TextBox3.Text
				If DIVAForm.CheckBox8.Checked Then
					optimize_f = optimize_f + " max_a=" + DIVAForm.TextBox5.Text
				Else
					optimize_f = optimize
				End If
			End If
			optimize_f = optimize_f + ";"
			optimize = optimize + ";"
            CmdBox.AppendText("Using command: " + optimize + vbCrLf)
        End If
	End Sub
	Public Sub Enable_Buttun()
		If Me.InvokeRequired Then
			Me.Invoke(New MethodInvoker(AddressOf Enable_Buttun))
		Else
			If Enable_BBM Then
				'MsgBox("The Only One tree in the tree dataset will be loaded as the final tree!", MsgBoxStyle.Information)
				Enable_BBM = False
			End If
			LoadTreesDataToolStripMenuItem.Enabled = False
			QuickLoadToolStripMenuItem.Enabled = False
			LoadOneTreeToolStripMenuItem.Enabled = False
			AddTreesDataToolStripMenuItem.Enabled = True
			LoadDistrutionToolStripMenuItem.Enabled = True
			LoadFinalTreeToolStripMenuItem.Enabled = True
			LoadDataToolStripMenuItem.Enabled = True
			RandomTreesToolStripMenuItem.Enabled = True
			TreeDataSetToolStripMenuItem.Enabled = True
		End If
	End Sub
    Public Sub Make_Taxon_List()
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf Make_Taxon_List))
        Else
            Dim Temp_list() As String = taxon_line.Replace("(", ",").Replace(")", ",").Replace(";", "").Replace("'", "").Replace("""", "").Split(New Char() {","c})
            Dim temp_Item_Array As Integer = 0
            For Each i As String In Temp_list
                If i <> "" Then
                    dtView.AllowNew = True
                    If (temp_Item_Array = 0 And dtView.Count = 1) = False Then
                        dtView.AddNew()
                    End If
                    dtView.AllowEdit = True
                    Dim newrow(2) As String
                    newrow(0) = dtView.Count
                    newrow(1) = i
                    newrow(2) = ""
                    dtView.Item(temp_Item_Array).Row.ItemArray = newrow
                    temp_Item_Array += 1
                End If
            Next
            taxon_num = dtView.Count
            If taxon_num > 64 Then
                DIVAForm.CheckBox2.Checked = False
            End If
            dtView.AllowNew = False
        End If
    End Sub
    Public Sub disable_Buttun()
		If Me.InvokeRequired Then
			Me.Invoke(New MethodInvoker(AddressOf disable_Buttun))
		Else

			OriginalMethodsToolStripMenuItem.Enabled = False
			StatisticalMethodsToolStripMenuItem.Enabled = False
			TraitsEvolutionToolStripMenuItem.Enabled = False
            ModelTestToolStripMenuItem.Enabled = False
            ComparisonToolStripMenuItem.Enabled = False
            OtherToolStripMenuItem.Enabled = False
            FinalTreeBox.Text = ""
		End If
	End Sub
	Public Sub Enable_Buttun1()
		If Me.InvokeRequired Then
			Me.Invoke(New MethodInvoker(AddressOf Enable_Buttun1))
		Else
			OriginalMethodsToolStripMenuItem.Enabled = True
			StatisticalMethodsToolStripMenuItem.Enabled = True
			TraitsEvolutionToolStripMenuItem.Enabled = True
            ModelTestToolStripMenuItem.Enabled = True
            ComparisonToolStripMenuItem.Enabled = True
            OtherToolStripMenuItem.Enabled = True
            'FinalTreeBox.Text = tree_path
        End If
	End Sub
	Public Sub Enable_Buttun2()
		If Me.InvokeRequired Then
			Me.Invoke(New MethodInvoker(AddressOf Enable_Buttun2))
		Else
			OriginalMethodsToolStripMenuItem.Enabled = True
			StatisticalMethodsToolStripMenuItem.Enabled = True
			TraitsEvolutionToolStripMenuItem.Enabled = True
			ModelTestToolStripMenuItem.Enabled = True
			OmittedTaxaToolStripMenuItem1.Enabled = True
            FormatedTreeToolStripMenuItem.Enabled = True

            ComparisonToolStripMenuItem.Enabled = True
            OtherToolStripMenuItem.Enabled = True
        End If
	End Sub
	Public Sub Enable_Windows()
		If Me.InvokeRequired Then
			Me.Invoke(New MethodInvoker(AddressOf Enable_Windows))
		Else
			FileToolStripMenuItem.Enabled = True
			GraphicToolStripMenuItem.Enabled = True
            ProcessToolStripMenuItem.Enabled = True
            ComparisonToolStripMenuItem.Enabled = True
            OtherToolStripMenuItem.Enabled = True
			GroupBox1.Enabled = True
			DataGridView1.Enabled = True
			Me.Refresh()
		End If

	End Sub
	Public Sub Disable_Windows()
		If Me.InvokeRequired Then
			Me.Invoke(New MethodInvoker(AddressOf Disable_Windows))
		Else
			FileToolStripMenuItem.Enabled = False
			GraphicToolStripMenuItem.Enabled = False
            ProcessToolStripMenuItem.Enabled = False
            ComparisonToolStripMenuItem.Enabled = False
            OtherToolStripMenuItem.Enabled = False
			GroupBox1.Enabled = False
			DataGridView1.Enabled = False
		End If
	End Sub
    Public Function load_names(ByVal ext_name As String) As Integer
        Try
            Dim line As String = ""
            File.Copy(tree_path, root_path + "temp" + path_char + "import.trees", True)
            Dim CopyFileInfo As New FileInfo(root_path + "temp" + path_char + "import.trees")
            CopyFileInfo.Attributes = FileAttributes.Normal

            'tree_path = root_path + "temp" + path_char + "import.trees"
            Dim rt As New StreamReader(tree_path)
            line = rt.ReadLine
            Do While line Is Nothing = False
                Do
                    If line.StartsWith("	") Or line.StartsWith(" ") Then
                        line = line.Remove(0, 1)
                    Else
                        Exit Do
                    End If
                Loop
                If ext_name.ToUpper = "NEX" Then
                    If line.Replace("	", "").ToUpper.StartsWith("MATRIX") And dtView.Count < 1 Then
                        dtView.AllowNew = True
                        dtView.AllowEdit = True

                        Dim temp_Item_Array As Integer = 0
                        Do
                            line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                            If line.Length > 0 Then
                                Do
                                    If line.StartsWith("	") Or line.StartsWith(" ") Then
                                        line = line.Remove(0, 1)
                                    Else
                                        Exit Do
                                    End If
                                Loop
                                Dim TRANSLATE As String = line.Replace(";", "").Substring(0, line.Replace(";", "").LastIndexOf(" ") + 1).Replace("'", "").Replace(" ", "_")

                                If (temp_Item_Array = 0 And dtView.Count = 1) = False Then
                                    dtView.AddNew()
                                End If
                                Dim newrow(2) As String
                                newrow(0) = dtView.Count
                                newrow(1) = TRANSLATE
                                newrow(2) = ""
                                dtView.Item(temp_Item_Array).Row.ItemArray = newrow
                                temp_Item_Array += 1
                            End If
                            line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                        Loop Until line.IndexOf(";") >= 0
                        If line.Replace("	", "").Replace(" ", "").Length > 1 Then
                            Do
                                If line.StartsWith("	") Or line.StartsWith(" ") Then
                                    line = line.Remove(0, 1)
                                Else
                                    Exit Do
                                End If
                            Loop
                            Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                            If (temp_Item_Array = 0 And dtView.Count = 1) = False Then
                                dtView.AddNew()
                            End If
                            Dim newrow(2) As String
                            newrow(0) = dtView.Count
                            newrow(1) = TRANSLATE(0).Replace("'", "")
                            newrow(2) = ""
                            dtView.Item(temp_Item_Array).Row.ItemArray = newrow
                            temp_Item_Array += 1
                        End If
                        dtView.AllowNew = False
                        Exit Do
                    End If
                End If

                If line.Replace("	", "").ToUpper.StartsWith("TRANSLATE") Then
                    dtView.AllowNew = True
                    dtView.AllowEdit = True
                    line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                    Dim temp_Item_Array As Integer = 0
                    Do
                        If line.Length > 0 Then
                            Do
                                If line.StartsWith("	") Or line.StartsWith(" ") Then
                                    line = line.Remove(0, 1)
                                Else
                                    Exit Do
                                End If
                            Loop
                            Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})

                            If (temp_Item_Array = 0 And dtView.Count = 1) = False Then
                                dtView.AddNew()
                            End If
                            Dim newrow(2) As String
                            newrow(0) = dtView.Count
                            newrow(1) = TRANSLATE(1).Replace("'", "")
                            newrow(2) = ""
                            dtView.Item(temp_Item_Array).Row.ItemArray = newrow
                            temp_Item_Array += 1
                        End If
                        line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                    Loop Until line.IndexOf(";") >= 0
                    If line.Replace("	", "").Replace(" ", "").Length > 1 Then
                        Do
                            If line.StartsWith("	") Or line.StartsWith(" ") Then
                                line = line.Remove(0, 1)
                            Else
                                Exit Do
                            End If
                        Loop
                        Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                        If (temp_Item_Array = 0 And dtView.Count = 1) = False Then
                            dtView.AddNew()
                        End If
                        Dim newrow(2) As String
                        newrow(0) = dtView.Count
                        newrow(1) = TRANSLATE(1).Replace("'", "")
                        newrow(2) = ""
                        dtView.Item(temp_Item_Array).Row.ItemArray = newrow
                        temp_Item_Array += 1
                    End If
                    dtView.AllowNew = False
                    Exit Do
                End If
                line = rt.ReadLine()
            Loop
            rt.Close()
            taxon_num = dtView.Count
            If taxon_num > 64 Then
                DIVAForm.CheckBox2.Checked = False
            End If
            config_SDIVA_node = ""
            Return 0
        Catch ex As Exception
            MsgBox("Error 5: Cannot format the trees!")
            Enable_Windows()
            Return 1
        End Try
    End Function
    Public Function load_trees() As Integer
        Try
            Dim line As String = ""
            Dim rt As StreamReader
            Dim B_WT As StreamWriter
            Dim P_WT As StreamWriter
            Dim have_translate As Boolean = False

            tree_filename = tree_path.Substring(tree_path.Replace("/", "\").LastIndexOf("\") + 1)
            If tree_filename.Contains(".") Then
                tree_filename = tree_filename.Substring(0, tree_filename.LastIndexOf("."))
            End If

            Dim name_wt As StreamWriter


            rt = New StreamReader(tree_path)
            If B_Tree_File = "" Then
                B_Tree_File = "temp" + path_char + format_time(Date.Now.ToString) + "_B.tre"
                P_Tree_File = "temp" + path_char + format_time(Date.Now.ToString) + "_P.tre"
                omittedtree = B_Tree_File
                B_WT = New StreamWriter(root_path + B_Tree_File, False)
                P_WT = New StreamWriter(root_path + P_Tree_File, False)
            Else
                B_WT = New StreamWriter(root_path + B_Tree_File, True)
                P_WT = New StreamWriter(root_path + P_Tree_File, True)
            End If
            name_wt = New StreamWriter(root_path + "temp\gene_name.txt", True)
            Dim name_count As Integer = 0

            Dim wt_clean_num As New StreamWriter(root_path + "temp" + path_char + "clean_num.trees", True)
            Dim wt_clean_num_P As New StreamWriter(root_path + "temp" + path_char + "clean_num_p.trees", True)
            line = rt.ReadLine

            Dim f_t_name(,) As String
            ReDim f_t_name(dtView.Count - 1, 1)
            Process_ID = 0
            Dim skip_trees As Integer = 0
            Do While line Is Nothing = False

                Do
                    If line.StartsWith("	") Or line.StartsWith(" ") Then
                        line = line.Remove(0, 1)
                    Else
                        Exit Do
                    End If
                Loop



                If line.Replace("	", "").ToUpper.StartsWith("TRANSLATE") Then
                    line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                    Dim name_num As Integer = 0
                    Do

                        If line.Length > 0 Then
                            Do
                                If line.StartsWith("	") Or line.StartsWith(" ") Then
                                    line = line.Remove(0, 1)
                                Else
                                    Exit Do
                                End If
                            Loop
                            Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                            f_t_name(name_num, 0) = TRANSLATE(0)
                            f_t_name(name_num, 1) = TRANSLATE(1).Replace("'", "")
                            name_num = name_num + 1
                        End If
                        line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                    Loop Until line.Contains(";")
                    If line.Replace("	", "").Replace(" ", "").Length > 1 Then
                        Do
                            If line.StartsWith("	") Or line.StartsWith(" ") Then
                                line = line.Remove(0, 1)
                            Else
                                Exit Do
                            End If
                        Loop
                        Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                        f_t_name(name_num, 0) = TRANSLATE(0)
                        f_t_name(name_num, 1) = TRANSLATE(1).Replace("'", "")
                        name_num = name_num + 1
                    End If
                End If

                If line.Replace("	", "").ToUpper.StartsWith("TREE") Or line.Replace("	", "").ToUpper.StartsWith("(") Then
                    Do While line.Contains(";") = False
                        Dim next_tree_line As String = rt.ReadLine
                        If next_tree_line <> "" Then
                            line = line + next_tree_line
                        End If
                    Loop
                    If skip_trees < CInt(BeforeLoad.Text) Then
                        skip_trees += 1
                        line = rt.ReadLine()
                        Continue Do
                    End If
                    Dim tree_Temp As String = line.Substring(line.IndexOf("("), line.Length - line.IndexOf("(")).Replace("'", "")
                    Dim tree_complete As String = ""
                    Dim tree_sdec As String = ""
                    Dim is_sym As Boolean = False
                    Dim is_kh As Boolean = False
                    Dim is_sym1 As Boolean = False
                    For k As Integer = 0 To tree_Temp.Length - 1
                        Dim tree_chr As Char = tree_Temp(k)
                        If tree_chr = "[" Then
                            is_sym1 = True
                            is_sym = True
                        End If
                        If tree_chr = "]" Then
                            is_sym1 = False
                        End If
                        If tree_chr = ":" Then
                            is_sym = True
                            is_kh = False
                        End If
                        If (tree_chr = "," Or tree_chr = "(" Or tree_chr = ")") And is_sym1 = False Then
                            is_sym = False
                            is_kh = False
                        End If
                        If tree_chr = ")" And is_sym1 = False Then
                            tree_complete &= tree_chr.ToString
                            tree_sdec &= tree_chr.ToString
                            is_sym = True
                            is_kh = True
                        End If
                        If is_sym = False Then
                            tree_complete &= tree_chr.ToString
                        End If
                        If is_sym1 = False And tree_chr <> "]" And is_kh = False Then
                            tree_sdec &= tree_chr.ToString
                        End If
                    Next
                    If dtView.Count <= 1 Then
                        taxon_line = tree_complete
                        Make_Taxon_List()
                    End If
                    Dim outgroup_str As String = ""
                    Dim isbase_three As Boolean = True
                    If tree_complete.Replace("(", "").Length - tree_complete.Replace(",", "").Length = 1 Then
                        Dim tree_poly() As Char = tree_complete
                        ReDim tree_char(taxon_num * 4)
                        tree_complete = ""
                        Dim char_id As Integer = 0
                        Dim l_c As Integer = 0
                        Dim r_c As Integer = 0
                        Dim dh As Integer = 0
                        Dim last_symb As Boolean = True
                        For i As Integer = 0 To tree_poly.Length - 1
                            Select Case tree_poly(i)
                                Case "("
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True

                                Case ")"
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True
                                Case ","
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True
                                Case Else
                                    If last_symb Then
                                        char_id += 1
                                        tree_char(char_id) = tree_poly(i)
                                        last_symb = False
                                    Else
                                        tree_char(char_id) += tree_poly(i)
                                    End If
                            End Select
                        Next
                        Dim three_clade_id(2) As Integer
                        three_clade_id(0) = 0
                        three_clade_id(1) = 0
                        three_clade_id(2) = 0
                        For i As Integer = 1 To tree_char.Length - 1
                            If tree_char(i) = "(" Then
                                l_c = l_c + 1
                            End If
                            If tree_char(i) = ")" Then
                                three_clade_id(2) = i
                            End If
                            If tree_char(i) = "," Then
                                dh = dh + 1
                            End If
                            If dh = l_c + 1 Then
                                If three_clade_id(1) = 0 Then
                                    dh = 0
                                    l_c = 0
                                End If
                                three_clade_id(1) = i
                            End If
                        Next
                        If dh <> l_c Then
                            isbase_three = False
                        End If
                        dh = 0
                        l_c = 0
                        For i As Integer = 0 To three_clade_id(2) - 1
                            If tree_char(three_clade_id(2) - i) = ")" Then
                                r_c = r_c + 1
                            End If
                            If tree_char(three_clade_id(2) - i) = "," Then
                                dh = dh + 1
                            End If
                            If dh = r_c + 1 Then
                                If three_clade_id(0) = 0 Then
                                    dh = 0
                                    r_c = 0
                                End If
                                three_clade_id(0) = three_clade_id(2) - i
                            End If
                        Next
                        If dh <> r_c Then
                            isbase_three = False
                        End If
                        dh = 0
                        r_c = 0
                        For i As Integer = 0 To three_clade_id(2) - 1
                            Select Case tree_char(i)
                                Case "("
                                    l_c += 1
                                Case ","
                                    dh = dh + 1
                                Case ")"
                                    r_c += 1
                                Case Else

                            End Select
                            If i = three_clade_id(0) Then
                                If l_c <> dh Or dh <> r_c + 1 Then
                                    isbase_three = False
                                End If
                            End If
                            If i = three_clade_id(1) Then
                                If l_c <> dh - 1 Or dh - 1 <> r_c + 1 Then
                                    isbase_three = False
                                End If
                            End If
                        Next
                        If isbase_three Then
                            If three_clade_id(2) - three_clade_id(1) <= three_clade_id(0) - 1 Then
                                If three_clade_id(2) - three_clade_id(1) <= three_clade_id(1) - three_clade_id(0) Then
                                    tree_complete = "("
                                    For i As Integer = 1 To three_clade_id(1) - 1
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += "),"
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ";"
                                Else
                                    tree_complete = "("
                                    For i As Integer = three_clade_id(0) To three_clade_id(1) - 1
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ","
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ");"
                                End If
                            Else
                                If three_clade_id(0) - 1 <= three_clade_id(1) - three_clade_id(0) Then
                                    tree_complete = ""
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += "("
                                    For i As Integer = three_clade_id(0) + 1 To three_clade_id(2)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ");"
                                Else
                                    tree_complete = "("
                                    For i As Integer = three_clade_id(0) + 1 To three_clade_id(1) - 1
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ","
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ");"
                                End If
                            End If
                        Else
                            For i As Integer = 1 To three_clade_id(2)
                                tree_complete = tree_complete + tree_char(i)
                            Next
                        End If
                    End If
                    If tree_complete.EndsWith(";") = False Then
                        tree_complete = tree_complete + ";"
                    End If

                    If taxon_line <> "" Then
                        For i As Integer = 1 To dtView.Count
                            If dtView.Item(i - 1).Item(0).ToString <> "" And dtView.Item(i - 1).Item(1).ToString <> "" Then
                                tree_complete = tree_complete.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ",", "($%#" + dtView.Item(i - 1).Item(0).ToString + "$%#,")
                                tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ")", ",$%#" + dtView.Item(i - 1).Item(0).ToString + "$%#)")
                                tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ",", ",$%#" + dtView.Item(i - 1).Item(0).ToString + "$%#,")
                                tree_complete = tree_complete.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ":", "($%#" + dtView.Item(i - 1).Item(0).ToString + "$%#:")
                                tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ":", ",$%#" + dtView.Item(i - 1).Item(0).ToString + "$%#:")
                            End If
                        Next
                        tree_complete = tree_complete.Replace("$%#", "")
                    End If
                    If dtView.Count > 0 Then
                        If f_t_name.Length = dtView.Count * 2 Then
                            For i As Integer = 1 To dtView.Count
                                If f_t_name(i - 1, 0) <> "" And f_t_name(i - 1, 1) <> "" Then
                                    tree_complete = tree_complete.Replace("(" + f_t_name(i - 1, 0) + ",", "($%*" + f_t_name(i - 1, 1) + "$%*,")
                                    tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ")", ",$%*" + f_t_name(i - 1, 1) + "$%*)")
                                    tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ",", ",$%*" + f_t_name(i - 1, 1) + "$%*,")
                                    tree_complete = tree_complete.Replace("(" + f_t_name(i - 1, 0) + ":", "($%*" + f_t_name(i - 1, 1) + "$%*:")
                                    tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ":", ",$%*" + f_t_name(i - 1, 1) + "$%*:")
                                End If
                            Next
                        End If
                        For i As Integer = 1 To dtView.Count
                            tree_complete = tree_complete.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ",", "($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,")
                            tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ")", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*)")
                            tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ",", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,")
                            tree_complete = tree_complete.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ":", "($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:")
                            tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ":", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:")
                        Next
                        For i As Integer = 1 To dtView.Count
                            tree_complete = tree_complete.Replace("($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,", "(" + dtView.Item(i - 1).Item(0).ToString + ",")
                            tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*)", "," + dtView.Item(i - 1).Item(0).ToString + ")")
                            tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,", "," + dtView.Item(i - 1).Item(0).ToString + ",")
                            tree_complete = tree_complete.Replace("($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:", "(" + dtView.Item(i - 1).Item(0).ToString + ":")
                            tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:", "," + dtView.Item(i - 1).Item(0).ToString + ":")
                        Next
                    End If
                    tree_complete = tree_complete.Replace(" ", "")
                    If tree_sdec.Replace("(", "").Length - tree_sdec.Replace(",", "").Length = 1 Then
                        Dim tree_poly() As Char = tree_sdec
                        ReDim tree_char(taxon_num * 5)
                        tree_sdec = ""
                        Dim char_id As Integer = 0
                        Dim l_c As Integer = 0
                        Dim r_c As Integer = 0
                        Dim dh As Integer = 0
                        Dim last_symb As Boolean = True
                        For i As Integer = 0 To tree_poly.Length - 1
                            Select Case tree_poly(i)
                                Case "("
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True

                                Case ")"
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True
                                Case ","
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True
                                Case Else
                                    If last_symb Then
                                        char_id += 1
                                        tree_char(char_id) = tree_poly(i)
                                        last_symb = False
                                    Else
                                        tree_char(char_id) += tree_poly(i)
                                    End If
                            End Select
                        Next
                        Dim three_clade_id(2) As Integer
                        three_clade_id(0) = 0
                        three_clade_id(1) = 0
                        three_clade_id(2) = 0
                        For i As Integer = 1 To tree_char.Length - 1
                            If tree_char(i) = "(" Then
                                l_c = l_c + 1
                            End If
                            If tree_char(i) = ")" Then
                                three_clade_id(2) = i
                            End If
                            If tree_char(i) = "," Then
                                dh = dh + 1
                            End If
                            If dh = l_c + 1 Then
                                If three_clade_id(1) = 0 Then
                                    dh = 0
                                    l_c = 0
                                End If
                                three_clade_id(1) = i
                            End If
                        Next
                        If dh <> l_c Then
                            isbase_three = False
                        End If
                        dh = 0
                        l_c = 0
                        For i As Integer = 0 To three_clade_id(2) - 1
                            If tree_char(three_clade_id(2) - i) = ")" Then
                                r_c = r_c + 1
                            End If
                            If tree_char(three_clade_id(2) - i) = "," Then
                                dh = dh + 1
                            End If
                            If dh = r_c + 1 Then
                                If three_clade_id(0) = 0 Then
                                    dh = 0
                                    r_c = 0
                                End If
                                three_clade_id(0) = three_clade_id(2) - i
                            End If
                        Next
                        If dh <> r_c Then
                            isbase_three = False
                        End If
                        dh = 0
                        r_c = 0
                        For i As Integer = 0 To three_clade_id(2) - 1
                            Select Case tree_char(i)
                                Case "("
                                    l_c += 1
                                Case ","
                                    dh = dh + 1
                                Case ")"
                                    r_c += 1
                                Case Else

                            End Select
                            If i = three_clade_id(0) Then
                                If l_c <> dh Or dh <> r_c + 1 Then
                                    isbase_three = False
                                End If
                            End If
                            If i = three_clade_id(1) Then
                                If l_c <> dh - 1 Or dh - 1 <> r_c + 1 Then
                                    isbase_three = False
                                End If
                            End If
                        Next
                        If isbase_three Then
                            If three_clade_id(2) - three_clade_id(1) <= three_clade_id(0) - 1 Then
                                If three_clade_id(2) - three_clade_id(1) <= three_clade_id(1) - three_clade_id(0) Then
                                    tree_sdec = "("
                                    For i As Integer = 1 To three_clade_id(1) - 1
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += "):0,"
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ";"
                                Else
                                    tree_sdec = "("
                                    For i As Integer = three_clade_id(0) To three_clade_id(1) - 1
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ","
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ");"
                                End If
                            Else
                                If three_clade_id(0) - 1 <= three_clade_id(1) - three_clade_id(0) Then
                                    tree_sdec = ""
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += "("
                                    For i As Integer = three_clade_id(0) + 1 To three_clade_id(2)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ");"
                                Else
                                    tree_sdec = "("
                                    For i As Integer = three_clade_id(0) + 1 To three_clade_id(1) - 1
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ","
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ");"
                                End If
                            End If
                        Else
                            For i As Integer = 1 To three_clade_id(2)
                                tree_sdec = tree_sdec + tree_char(i)
                            Next
                        End If
                    End If

                    If tree_sdec.EndsWith(";") = False Then
                        tree_sdec = tree_sdec + ";"
                    End If

                    If taxon_line <> "" Then
                        For i As Integer = 1 To dtView.Count
                            If dtView.Item(i - 1).Item(0).ToString <> "" And dtView.Item(i - 1).Item(1).ToString <> "" Then
                                tree_sdec = tree_sdec.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ",", "($%#" + dtView.Item(i - 1).Item(0).ToString + "$%#,")
                                tree_sdec = tree_sdec.Replace("," + dtView.Item(i - 1).Item(1).ToString + ")", ",$%#" + dtView.Item(i - 1).Item(0).ToString + "$%#)")
                                tree_sdec = tree_sdec.Replace("," + dtView.Item(i - 1).Item(1).ToString + ",", ",$%#" + dtView.Item(i - 1).Item(0).ToString + "$%#,")
                                tree_sdec = tree_sdec.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ":", "($%#" + dtView.Item(i - 1).Item(0).ToString + "$%#:")
                                tree_sdec = tree_sdec.Replace("," + dtView.Item(i - 1).Item(1).ToString + ":", ",$%#" + dtView.Item(i - 1).Item(0).ToString + "$%#:")
                            End If
                        Next
                        tree_sdec = tree_sdec.Replace("$%#", "")
                    End If
                    If dtView.Count > 0 Then
                        If f_t_name.Length = dtView.Count * 2 Then
                            For i As Integer = 1 To dtView.Count
                                If f_t_name(i - 1, 0) <> "" And f_t_name(i - 1, 1) <> "" Then
                                    tree_sdec = tree_sdec.Replace("(" + f_t_name(i - 1, 0) + ",", "($%*" + f_t_name(i - 1, 1) + "$%*,")
                                    tree_sdec = tree_sdec.Replace("," + f_t_name(i - 1, 0) + ")", ",$%*" + f_t_name(i - 1, 1) + "$%*)")
                                    tree_sdec = tree_sdec.Replace("," + f_t_name(i - 1, 0) + ",", ",$%*" + f_t_name(i - 1, 1) + "$%*,")
                                    tree_sdec = tree_sdec.Replace("(" + f_t_name(i - 1, 0) + ":", "($%*" + f_t_name(i - 1, 1) + "$%*:")
                                    tree_sdec = tree_sdec.Replace("," + f_t_name(i - 1, 0) + ":", ",$%*" + f_t_name(i - 1, 1) + "$%*:")
                                End If
                            Next
                        End If
                        For i As Integer = 1 To dtView.Count
                            tree_sdec = tree_sdec.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ",", "($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,")
                            tree_sdec = tree_sdec.Replace("," + dtView.Item(i - 1).Item(1).ToString + ")", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*)")
                            tree_sdec = tree_sdec.Replace("," + dtView.Item(i - 1).Item(1).ToString + ",", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,")
                            tree_sdec = tree_sdec.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ":", "($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:")
                            tree_sdec = tree_sdec.Replace("," + dtView.Item(i - 1).Item(1).ToString + ":", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:")
                        Next
                        For i As Integer = 1 To dtView.Count
                            tree_sdec = tree_sdec.Replace("($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,", "(" + dtView.Item(i - 1).Item(0).ToString + ",")
                            tree_sdec = tree_sdec.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*)", "," + dtView.Item(i - 1).Item(0).ToString + ")")
                            tree_sdec = tree_sdec.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,", "," + dtView.Item(i - 1).Item(0).ToString + ",")
                            tree_sdec = tree_sdec.Replace("($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:", "(" + dtView.Item(i - 1).Item(0).ToString + ":")
                            tree_sdec = tree_sdec.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:", "," + dtView.Item(i - 1).Item(0).ToString + ":")
                        Next
                    End If
                    tree_sdec = tree_sdec.Replace(" ", "")

                    If tree_complete.Replace("(", "").Length <> tree_complete.Replace(")", "").Length Then
                        MsgBox("Error 10. missing parentheses in tree! Please check you tree file!")
                        Try
                            rt.Close()
                            B_WT.Close()
                            P_WT.Close()
                            name_wt.Close()
                            Enable_Windows()
                            Enable_Buttun()
                        Catch ex As Exception

                        End Try
                        Return 1
                    End If
                    If tree_sdec.Replace("):", "").Length = tree_sdec.Length - 2 Then
                        tree_sdec = tree_sdec.Replace("):0", ")")
                    End If
                    If tree_complete.Replace("(", "").Length = tree_complete.Replace(",", "").Length Then
                        wt_clean_num.WriteLine(tree_sdec)
                        wt_clean_num_P.WriteLine(tree_sdec)
                        B_WT.WriteLine(tree_complete)
                        P_WT.WriteLine(tree_complete)
                        Tree_Num_B += 1
                        Process_Gen1 = Tree_Num_B
                    Else
                        wt_clean_num_P.WriteLine(tree_sdec)
                        P_WT.WriteLine(tree_complete)
                    End If
                    Tree_Num_P += 1
                    name_count += 1
                    ReDim Preserve gene_names(gene_names.Length)
                    gene_names(UBound(gene_names)) = tree_filename + "_" + name_count.ToString
                    name_wt.WriteLine(tree_filename + "_" + name_count.ToString)
                    Process_Gen = Tree_Num_P
                End If
                line = rt.ReadLine()
            Loop

            Me.Invoke(TB1, New Object() {Tree_Num_B.ToString})
            Me.Invoke(TB3, New Object() {Tree_Num_P.ToString})
            Process_Text += "Load " + tree_path.Substring(tree_path.Replace("/", "\").LastIndexOf("\") + 1) + " Successfully!" + vbCrLf
            Process_ID = -1
            rt.Close()
            B_WT.Close()
            P_WT.Close()
            name_wt.Close()
            wt_clean_num.Close()
            wt_clean_num_P.close()
            If Tree_Num_P = 1 Then
                load_final_trees()
                Enable_BBM = True
            End If
            If files_complete Then
                Enable_Windows()
                Enable_Buttun()
            End If

            Return 0
        Catch ex As Exception
            files_complete = True
            MsgBox(ex.ToString)
            MsgBox("Error 6. Cannot format the trees!")
            Enable_Windows()
            Return 1
        End Try
    End Function

    Public Sub OpenData()
        Dim taxon_table As New System.Data.DataTable
        taxon_table.TableName = "Taxon Table"
        Dim Column_ID As New System.Data.DataColumn("ID", System.Type.GetType("System.Int32"))
        Dim Column_Taxon As New System.Data.DataColumn("Name")
        Dim Column_Distrution As New System.Data.DataColumn("State")
        taxon_table.Columns.Add(Column_ID)
        taxon_table.Columns.Add(Column_Taxon)
        taxon_table.Columns.Add(Column_Distrution)
        Taxon_Dataset.Tables.Add(taxon_table)

        dtView = Taxon_Dataset.Tables("Taxon Table").DefaultView
        dtView.AllowNew = False
        dtView.AllowDelete = False
        dtView.AllowEdit = True
        DataGridView1.DataSource = dtView
        DataGridView1.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
        DataGridView1.Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable
        DataGridView1.Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable

        DataGridView1.Columns(0).ReadOnly = True
        DataGridView1.Columns(1).ReadOnly = True
        Dim outgroup As New DataGridViewCheckBoxColumn
        outgroup.HeaderText = "Select"
        DataGridView1.Columns.Insert(0, outgroup)
        DataGridView1.Columns(1).Width = 50
        DataGridView1.Columns(0).Width = 50

        Dim node_table As New System.Data.DataTable
        node_table.TableName = "NodeTable"
        Dim Node_ID As New System.Data.DataColumn("Node ID")
        Dim Node_Member As New System.Data.DataColumn("Taxon ID")
        node_table.Columns.Add(Node_ID)
        node_table.Columns.Add(Node_Member)
        Node_Dataset.Tables.Add(node_table)

        nodeView = Node_Dataset.Tables("NodeTable").DefaultView
        nodeView.AllowNew = False
        nodeView.AllowDelete = False
        nodeView.AllowEdit = True
    End Sub
    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles BurninBox.KeyPress
        If IsNumeric(e.KeyChar) Or e.KeyChar = Chr(Keys.Back) Then
            Return
        End If
        e.Handled = True
    End Sub
    Public Sub runDIVA()
        Me.Invoke(RT1, New Object() {vbCrLf + "*******************************************" + vbCrLf})
        Me.Invoke(RT1, New Object() {"*Statistical Dispersal-Vicariance Analysis*" + vbCrLf})
        Me.Invoke(RT1, New Object() {"*******************************************" + vbCrLf})
        Me.Invoke(RT1, New Object() {"Process begin at " + Date.Now.ToString + vbCrLf})
        Make_Optimize()
        Dim rt As New StreamReader(root_path + B_Tree_File)
        Dim wbatch As New StreamWriter(root_path + "temp" + path_char + "diva.proc")
        Dim burn_in As Integer = CInt(BurninBox.Text)
        Do While burn_in > 0
            rt.ReadLine()
            burn_in = burn_in - 1
        Loop
        Dim current_tree As String
        Begin_Show = True
        Dim seed As Integer = DateTime.Now.Millisecond
        If Global_seed <> "20180127" Then
            seed = Global_seed
        End If
        Dim rand As New System.Random(seed)
        Dim is_random As Boolean = False
        Dim random_num As Integer = 0
        Dim random_array(0) As Integer
        For i As Integer = 0 To muti_threads_DIVA - 1
            Dim sw As New StreamWriter(root_path + "temp" + path_char + "SDIVA_" + i.ToString + ".proc", False)
            sw.Close()
        Next
        Try
            Process_Int = 0
            Process_ID = 1
            Me.Invoke(RT2_S, New Object() {"Making command ..."})
            Dim Analyse_trees As New StreamWriter(root_path + "temp" + path_char + "random_trees.tre", False)
            If excludeline <> "" Then
                wbatch.WriteLine(excludeline)
            End If

            For t As Integer = CInt(BurninBox.Text) + 1 To CInt(TreeBox.Text)

                If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
                    Dim random_w As New StreamWriter(root_path + "temp" + path_char + "random_num.txt", False)
                    ReDim random_array(CInt(RandomTextBox.Text))
                    random_w.WriteLine(RandomTextBox.Text)
                    is_random = True
r2:                 If random_num < CInt(RandomTextBox.Text) Then
                        random_num = random_num + 1
                        t = rand.Next(CInt(BurninBox.Text) + 1, CInt(TreeBox.Text))
                        random_array(random_num) = t
                        random_w.WriteLine(t.ToString)
                        GoTo n_r1
                    End If
                    random_w.Close()
                    Exit For
                End If
n_r1:
                If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
                    Dim random_rt As New StreamReader(root_path + B_Tree_File)
                    For i As Integer = 1 To t - 1
                        random_rt.ReadLine()
                    Next
                    current_tree = random_rt.ReadLine()
                    random_rt.Close()

                Else
                    current_tree = rt.ReadLine()
                End If
                Analyse_trees.WriteLine(current_tree)
                Dim has_f As Boolean = False
                DeleteFiles(root_path + "temp", ".diva")
                DistributionLine = "distribution"
                For i As Integer = 1 To dtView.Count
                    DistributionLine = DistributionLine + " " + dtView.Item(i - 1).Item(state_index).ToString
                Next
                If has_f Then
                    DistributionLine = DistributionLine + " " + config_SDIVA_omitted
                End If
                DistributionLine = DistributionLine + ";"
                If is_random = False Or (is_random And Array.IndexOf(random_array, t) = random_num) Then
                    wbatch.WriteLine("output temp" + path_char + t.ToString + ".diva;")
                    wbatch.WriteLine("tree " + current_tree)
                    wbatch.WriteLine(DistributionLine)
                    wbatch.WriteLine(optimize)
                    Dim sw As New StreamWriter(root_path + "temp" + path_char + "SDIVA_" + (t Mod muti_threads_DIVA).ToString + ".proc", True)
                    sw.WriteLine("output " + t.ToString + ".diva;")
                    sw.WriteLine("tree " + current_tree)
                    sw.WriteLine(DistributionLine)
                    sw.WriteLine(optimize)
                    sw.Close()
                End If
                If is_random Then
                    PV_SUM = CInt(RandomTextBox.Text)
                    Process_Int = CInt(random_num / PV_SUM * 10000)
                    GoTo r2
                End If
                PV_SUM = (CInt(TreeBox.Text) - CInt(BurninBox.Text))
                Process_Int = CInt((t - CInt(BurninBox.Text)) / PV_SUM * 10000)
            Next
            Process_Int = 0
            Process_ID = -1
            Analyse_trees.Close()
        Catch ex As Exception
            MsgBox(ex.ToString)
            MsgBox("Cannot process the trees!")
            Exit Sub
        End Try

        Try
            If DIVAForm.CheckBox6.Checked Then
                If File.Exists(root_path + "temp" + path_char + "final.diva") Then
                    File.Delete(root_path + "temp" + path_char + "final.diva")
                End If
                DistributionLine = "distribution"
                For i As Integer = 1 To dtView.Count
                    DistributionLine = DistributionLine + " " + dtView.Item(i - 1).Item(state_index).ToString
                Next

                DistributionLine = DistributionLine + ";"
                Me.Invoke(RT1, New Object() {"Condensed tree used command: " + optimize_f + vbCrLf})
                wbatch.WriteLine("output temp" + path_char + "final.diva;")
                wbatch.WriteLine("tree " + final_tree)
                wbatch.WriteLine(DistributionLine)
                wbatch.WriteLine(fossilline)
                wbatch.WriteLine(optimize_f)
                Dim sw As New StreamWriter(root_path + "temp" + path_char + "SDIVA_0.proc", True)
                sw.WriteLine("output " + "final.diva;")
                sw.WriteLine("tree " + final_tree)
                sw.WriteLine(DistributionLine)
                sw.WriteLine(fossilline)
                sw.WriteLine(optimize_f)
                sw.Close()
            End If
        Catch ex As Exception
            wbatch.Close()
            MsgBox(ex.ToString)
            MsgBox("Cannot process the computed file!")
            Exit Sub
        End Try
        Process_Int = 0
        Process_ID = 1
        rt.Close()
        wbatch.WriteLine("output temp\DIVA.end;")
        wbatch.WriteLine("output temp\DIVA.end;")
        wbatch.Close()
        For i As Integer = 0 To muti_threads_DIVA - 1
            Dim sw As New StreamWriter(root_path + "temp" + path_char + "SDIVA_" + i.ToString + ".proc", True)
            sw.WriteLine("output DIVA_" + i.ToString + ".end;")
            sw.WriteLine("quit;")
            sw.Close()
        Next
        DeleteFiles(root_path + "temp", ".end")
        If DIVAForm.NumericUpDown3.Value >= 1 Then
            For i As Integer = 0 To muti_threads_DIVA - 1
                Dim wr3 As New StreamWriter(root_path + "temp\SDIVA_" + i.ToString + ".bat", False, System.Text.Encoding.Default)
                wr3.WriteLine("""" + root_path + "Plug-ins\DIVA.exe" + """" + " SDIVA_" + i.ToString + ".proc")
                wr3.Close()
            Next
            current_dir = Directory.GetCurrentDirectory
            Directory.SetCurrentDirectory(root_path + "temp\")
            For i As Integer = 0 To muti_threads_DIVA - 1
                Dim startInfo As New ProcessStartInfo
                startInfo.FileName = "SDIVA_" + i.ToString + ".bat"
                startInfo.WorkingDirectory = root_path + "temp"
                startInfo.UseShellExecute = False
                startInfo.CreateNoWindow = DIVAForm.CheckBox13.Checked
                startInfo.RedirectStandardOutput = DIVAForm.CheckBox13.Checked
                startInfo.RedirectStandardInput = DIVAForm.CheckBox13.Checked
                startInfo.RedirectStandardError = DIVAForm.CheckBox13.Checked
                startInfo.WindowStyle = ProcessWindowStyle.Minimized
                Process.Start(startInfo)
            Next
            Directory.SetCurrentDirectory(current_dir)
            Process_Int = 0
            Process_ID = 11
        Else
            Process_ID = 4
            Process_Int = 0
            Me.Invoke(RT2_S, New Object() {"Optimize ..."})
            Dim return_int As Integer = 0
            return_int = divapro("proc temp" + path_char + "diva.proc;" + vbCrLf, diva_gen)
            If return_int = 100 Then
                Me.Invoke(RT1, New Object() {"Tree Process completed!" + vbCrLf})
            Else
                Me.Invoke(RT1, New Object() {"Tree Process got error " + return_int.ToString + vbCrLf})
            End If
            Process_Int = 0
            Process_ID = 1

            Me.Invoke(RT1, New Object() {"Process end at " + Date.Now.ToString + vbCrLf})
            Me.Invoke(RT2_S, New Object() {"S-DIVA Analysis ..."})

            do_analysis()
            Me.Invoke(RT2_S, New Object() {""})
            Process_Text += vbCrLf + "Analysis end at " + Date.Now.ToString + vbCrLf
            Process_Int = 0
            Process_ID = -1
            StartTreeView = True
            tree_view_title = "S-DIVA Analysis Result"
        End If

    End Sub

    Public Function read_node(ByVal treeline As String, ByVal taxon_num As Integer) As Integer
        Dim l_c As Integer = 0
        Dim r_c As Integer = 0
        If taxon_num <= treeline.Length - treeline.Replace("(", "").Length Then
            MsgBox("Extra pair of parentheses '()'")
            CloseDataToolStripMenuItem_Click(0, System.EventArgs.Empty)
            Return -1
        End If
        '0,级别 1,左分支 2,右分支 3,长度
        ReDim node(taxon_num - 1, 3)
        ReDim tree_char(taxon_num * 5)
        Dim node_num As Integer = 0
        Dim i As Integer = 0
        Dim Temp_node() As Integer
        ReDim Temp_node(taxon_num - 1)
        For Each Temp_char As Char In treeline

            Select Case Temp_char
                Case "("
                    l_c = l_c + 1
                    i = i + 1
                Case ")"
                    r_c = r_c + 1
                    i = i + 1
                Case ","
                    node_num = node_num + 1
                    node(node_num, 0) = l_c - r_c
                    i = i + 1
                    Temp_node(node_num) = i
                Case Else
                    If tree_char(i) = "(" Or tree_char(i) = ")" Or tree_char(i) = "," Then
                        i = i + 1
                    End If
            End Select
            tree_char(i) = tree_char(i) + Temp_char
        Next

        For n As Integer = 1 To taxon_num - 1
            Dim node_str As String = ""
            l_c = 0
            r_c = 0
            i = Temp_node(n)
            Do
                i = i - 1
                Select Case tree_char(i)
                    Case "("
                        l_c = l_c + 1
                    Case ")"
                        r_c = r_c + 1
                    Case ","
                        If node(Array.IndexOf(Temp_node, i).ToString, 0) - node(n, 0) = 1 Then
                            node_str = Array.IndexOf(Temp_node, i).ToString
                        End If
                    Case Else
                        If l_c = r_c Then
                            node_str = "$" + tree_char(i) + "$"
                        End If
                End Select
            Loop Until l_c = r_c
            node(n, 1) = node_str
            l_c = 0
            r_c = 0
            i = Temp_node(n)
            node_str = ""
            Do
                i = i + 1
                Select Case tree_char(i)
                    Case "("
                        l_c = l_c + 1
                    Case ")"
                        r_c = r_c + 1
                    Case ","
                        If node(Array.IndexOf(Temp_node, i).ToString, 0) - node(n, 0) = 1 Then
                            node_str = Array.IndexOf(Temp_node, i).ToString
                        End If
                    Case Else
                        If l_c = r_c Then
                            node_str = "$" + tree_char(i) + "$"
                        End If

                End Select
            Loop Until l_c = r_c
            node(n, 2) = node_str

        Next
        For j As Integer = 1 To taxon_num - 1
            If node(j, 1).StartsWith("$") = False Then
                node(j, 1) = node_contain(node(j, 1))
            End If
            If node(j, 2).StartsWith("$") = False Then
                node(j, 2) = node_contain(node(j, 2))
            End If
        Next
        Return 0
    End Function
    Public Function node_contain(ByVal num As Integer) As String
        Dim contain As String = ""
        Dim left_clade As String = node(num, 1)
        Dim right_clade As String = node(num, 2)
        If left_clade.StartsWith("$") Then
            contain = contain + left_clade
        Else
            contain = contain + node_contain(left_clade)
        End If
        If right_clade.StartsWith("$") Then
            contain = contain + right_clade
        Else
            contain = contain + node_contain(right_clade)
        End If
        Return contain
    End Function
    '	Public Sub estimate_node()
    '		Me.Invoke(RT1, New Object() {"Analysis begin at " + Date.Now.ToString + vbCrLf})
    '		Dim r_num As Integer
    '		Dim use_r_file As Boolean = True
    '		'[检查文件完整性
    '		If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
    '			use_r_file = True
    '			Dim check_random_r As New StreamReader(root_path + "temp" + path_char + "random_num.txt")
    '			r_num = CInt(check_random_r.ReadLine)
    '			Me.Invoke(RTB, New Object() {r_num.ToString})
    '			Dim randline As String = check_random_r.ReadLine
    '			For i As Integer = 1 To r_num
    '				If File.Exists(root_path + "temp" + path_char + randline + ".diva") = False Then
    '					Me.Invoke(RT1, New Object() {"Missing DIVA results of tree " + randline + "! Cannot do the analysis!"})
    '					Enable_Windows()
    '					Exit Sub
    '				End If
    '			Next
    '			check_random_r.Close()
    '		Else
    '			For t As Integer = CInt(BurninBox.Text) + 1 To CInt(TreeBox.Text)
    '				Process_Int = CInt(10000 * (t - CInt(BurninBox.Text)) / (CInt(TreeBox.Text) - CInt(BurninBox.Text)))
    '				If File.Exists(root_path + "temp" + path_char + t.ToString + ".diva") = False Then
    '					Me.Invoke(RT1, New Object() {"Missing DIVA results of tree " + t.ToString + "! Cannot do the analysis!"})
    '					Enable_Windows()
    '					Exit Sub
    '				End If
    '			Next
    '		End If
    '		'检查文件完整性]

    '		Process_Int = 0
    '		'[建立节点数组
    '		Dim particular_node_clade As String = ""
    '		Dim particular_node_area As String = "#"
    '		Dim particular_node_probablily(1) As Integer
    '		particular_node_probablily(0) = 0
    '		particular_node_probablily(1) = 0
    '		Dim Tempnodearry() As String = (config_SDIVA_node.Replace(" ", "").Replace(",,", ",") + ",").Replace(",,", "").Split(New Char() {","c})
    '		For Each Tempstr As String In Tempnodearry
    '			If Tempstr <> "" Then
    '				If Tempstr.Contains("-") Then
    '					Dim Tempnum() As String = Tempstr.Split(New Char() {"-"c})
    '					For i As Integer = CInt(Tempnum(0)) To CInt(Tempnum(1))
    '						particular_node_clade = particular_node_clade + "#" + i.ToString
    '					Next
    '				Else
    '					particular_node_clade = particular_node_clade + "#" + Tempstr
    '				End If
    '			End If
    '		Next
    '		particular_node_clade = particular_node_clade.Replace("##", "#")
    '		If DIVAForm.CheckBox10.Checked And DIVAForm.CheckBox3.Checked Then
    '			particular_node_clade = "#" + (dtView.Count + 1).ToString + particular_node_clade
    '		End If
    '		Dim Tempnodearry1() As String = particular_node_clade.Split(New Char() {"#"c})
    '		Array.Sort(Tempnodearry1)
    '		particular_node_clade = "#"
    '		For Each Tempstr As String In Tempnodearry1
    '			If Tempstr <> "" Then
    '				particular_node_clade = particular_node_clade + "#" + Tempstr
    '			End If
    '		Next
    '		particular_node_clade = particular_node_clade.Replace("##", "#").Replace("##", "#")

    '		'建立节点数组]
    '		Dim node_line() As String
    '		ReDim node_line(dtView.Count - 2)
    '		'[统计分布地数据！
    '		'[跳过burnin
    '		Dim current_tree As String
    '		Dim burn_in As Integer = CInt(BurninBox.Text)
    '		Dim has_printrecs As Boolean = False

    '		Dim rt_1 As New StreamReader(root_path + B_Tree_File)
    '		burn_in = CInt(BurninBox.Text)
    '		Do While burn_in > 0
    '			rt_1.ReadLine()
    '			burn_in = burn_in - 1
    '		Loop
    '		If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
    '			rt_1.Close()
    '		End If

    '		'跳过burnin]
    '		Dim line As String = ""
    '		'循环处理
    '		Dim rand As New System.Random()
    '		Dim random_num As Integer = 0
    '		Dim r_num_a As Integer = 0

    '		For t As Integer = CInt(BurninBox.Text) + 1 To CInt(TreeBox.Text)
    '			If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
    '				If use_r_file Then
    '					Dim check_random_r As New StreamReader(root_path + "temp" + path_char + "random_num.txt")
    '					r_num_a = check_random_r.ReadLine()
    'r_a_3:              If r_num_a > 0 Then
    '						t = CInt(check_random_r.ReadLine())
    '						r_num_a = r_num_a - 1
    '						GoTo n_r_a_1
    '					End If
    '					check_random_r.Close()
    '					Exit For
    '				Else
    'r_a_2:              If random_num < CInt(RandomTextBox.Text) Then
    '						random_num = random_num + 1
    '						GoTo r_a_1
    '					End If
    '					Exit For
    '				End If

    '			End If
    '			GoTo n_r_a_1

    'r_a_1:      t = rand.Next(CInt(BurninBox.Text) + 1, CInt(TreeBox.Text) + 1)
    'n_r_a_1:    If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
    '                Dim random_rt As New StreamReader(root_path + B_Tree_File)
    '                For i As Integer = 1 To t - 1
    '                    random_rt.ReadLine()
    '                Next
    '                current_tree = random_rt.ReadLine()
    '                random_rt.Close()
    '            Else
    '                current_tree = rt_1.ReadLine()
    '			End If
    '			Dim has_x As Boolean = False
    '			Dim node_count As Integer = dtView.Count - 2
    '			If DIVAForm.CheckBox10.Checked And DIVAForm.CheckBox3.Checked Then
    '				If current_tree <> f_node(current_tree) Then
    '					current_tree = f_node(current_tree)
    '					has_x = True
    '					ReDim node_line(dtView.Count - 1)
    '					node_count = dtView.Count - 1
    '				End If
    '			End If



    '			Dim tree_Arr() As String
    '			tree_Arr = current_tree.Replace("(", "#").Replace(")", "#").Replace(",", "#").Split(New Char() {"#"c})

    '			Dim r_c_t As New StreamReader(root_path + "temp" + path_char + t.ToString + ".diva")
    '			line = r_c_t.ReadLine

    '			Dim Temp_final_area() As String
    '			Dim Temp_particular_node_area As String = "#"
    '			Temp_final_area = Temp_particular_node_area.Split(New Char() {"#"c})
    '			Dim reconstruction_num As Integer = 0
    '			'處理樹的個數
    '			particular_node_probablily(0) = particular_node_probablily(0) + 1
    '			Dim hasclade As Boolean = False
    '			Do
    '				'If line.ToUpper.StartsWith("optimal reconstruction".ToUpper) Then
    '				If line.ToUpper.StartsWith(P_mode.ToUpper) Then
    '					reconstruction_num = reconstruction_num + 1
    '					has_printrecs = True
    '					Dim l As Integer = 0
    '					Do
    '						line = r_c_t.ReadLine
    '						If line.StartsWith("node") Then
    '							node_line(l) = line
    '							l = l + 1
    '						Else
    '							node_line(l - 1) = node_line(l - 1) + line.Replace("     ", "  ")
    '						End If
    '					Loop Until line = ""


    '					For n As Integer = 0 To node_count
    '						Dim cladeline() As String
    '						cladeline = node_line(n).Substring(node_line(n).IndexOf("terminals ") + "terminals ".Length, node_line(n).IndexOf(")") - node_line(n).IndexOf("terminals ") - "terminals ".Length).Split(New Char() {"-"c})
    '						Dim Temparry() As String
    '						If Array.IndexOf(tree_Arr, cladeline(0)) > Array.IndexOf(tree_Arr, cladeline(1)) Then

    '							ReDim Temparry(Array.IndexOf(tree_Arr, cladeline(0)) - Array.IndexOf(tree_Arr, cladeline(1)))
    '							For i As Integer = Array.IndexOf(tree_Arr, cladeline(1)) To Array.IndexOf(tree_Arr, cladeline(0))
    '								Temparry(i - Array.IndexOf(tree_Arr, cladeline(1))) = tree_Arr(i)
    '							Next
    '						Else
    '							ReDim Temparry(Array.IndexOf(tree_Arr, cladeline(1)) - Array.IndexOf(tree_Arr, cladeline(0)))
    '							For i As Integer = Array.IndexOf(tree_Arr, cladeline(0)) To Array.IndexOf(tree_Arr, cladeline(1))
    '								Temparry(i - Array.IndexOf(tree_Arr, cladeline(0))) = tree_Arr(i)
    '							Next
    '						End If
    '						Array.Sort(Temparry)
    '						Dim Temp_clade As String = ""
    '						For Each Tempstr As String In Temparry
    '							If Tempstr <> "" Then
    '								Temp_clade = Temp_clade + "#" + Tempstr
    '							End If
    '						Next

    '						If particular_node_clade = Temp_clade Then
    '							hasclade = True
    '							'去除不应有的区域
    '							Dim Temp_area() As String = node_line(n).Substring(node_line(n).IndexOf(":") + 2, node_line(n).Length - node_line(n).IndexOf(":") - 2).Replace("  ", " ").Replace(" ", "#").Split(New Char() {"#"c})

    '							'排除区域结束

    '							For Each i As String In Temp_area
    '								If Array.IndexOf(Temp_final_area, i) >= 0 Then
    '									Temp_final_area(Array.IndexOf(Temp_final_area, i) + 1) = (CSng(Temp_final_area(Array.IndexOf(Temp_final_area, i) + 1)) + 1).ToString
    '								Else
    '									Temp_particular_node_area = "#"
    '									For Each k As String In Temp_final_area
    '										If k <> "" Then
    '											Temp_particular_node_area = Temp_particular_node_area + k + "#"
    '										End If
    '									Next
    '									Temp_particular_node_area = Temp_particular_node_area.Replace("##", "#")
    '									Temp_particular_node_area = Temp_particular_node_area + "#" + i + "#1#"
    '									Temp_particular_node_area = Temp_particular_node_area.Replace("##", "#")
    '									Temp_final_area = Temp_particular_node_area.Split(New Char() {"#"c})
    '								End If
    '							Next
    '						End If

    '					Next

    '				End If
    '				line = r_c_t.ReadLine
    '			Loop Until line Is Nothing
    '			r_c_t.Close()
    '			'[累加结果
    '			If hasclade Then
    '				particular_node_probablily(1) = particular_node_probablily(1) + 1
    '			End If
    '			If DIVAForm.CheckBox2.Checked = False Then
    '				reconstruction_num = Temp_final_area.Length / 2 - 1
    '			End If
    '			Temp_particular_node_area = "#"
    '			For Each i As String In Temp_final_area
    '				If i <> "" Then
    '					If IsNumeric(i) Then
    '						Temp_particular_node_area = Temp_particular_node_area + (CSng(i) / reconstruction_num).ToString + "#"
    '					Else
    '						Temp_particular_node_area = Temp_particular_node_area + i + "#"
    '					End If
    '				End If
    '			Next
    '			Temp_particular_node_area = Temp_particular_node_area.Replace("##", "#")

    '			Dim Temp_arry() As String = Temp_particular_node_area.Split(New Char() {"#"c})
    '			Dim Temp_arry1() As String = particular_node_area.Split(New Char() {"#"c})

    '			For j As Integer = 1 To Temp_arry.Length - 2 Step 2
    '				If Array.IndexOf(Temp_arry1, Temp_arry(j)) > 0 Then
    '					Temp_arry1(Array.IndexOf(Temp_arry1, Temp_arry(j)) + 1) = (CSng(Temp_arry1(Array.IndexOf(Temp_arry1, Temp_arry(j)) + 1)) + CSng(Temp_arry(j + 1))).ToString
    '				Else
    '					particular_node_area = "#"
    '					For Each k As String In Temp_arry1
    '						If k <> "" Then
    '							particular_node_area = particular_node_area + k + "#"
    '						End If
    '					Next
    '					particular_node_area = particular_node_area.Replace("##", "#")
    '					particular_node_area = particular_node_area + "#" + Temp_arry(j) + "#" + Temp_arry(j + 1) + "#"
    '					particular_node_area = particular_node_area.Replace("##", "#")
    '					Temp_arry1 = particular_node_area.Split(New Char() {"#"c})
    '				End If
    '			Next
    '			particular_node_area = "#"
    '			For Each k As String In Temp_arry1
    '				If k <> "" Then
    '					particular_node_area = particular_node_area + k + "#"
    '				End If
    '			Next
    '			particular_node_area = particular_node_area.Replace("##", "#")
    '			r_c_t.Close()
    '			'[进度条
    '			If CheckBox3.Checked And CInt(RandomTextBox.Text) Then
    '				If use_r_file Then
    '					Process_Int = CInt(10000 * (r_num - r_num_a) / r_num)
    '					GoTo r_a_3
    '				Else
    '					Process_Int = CInt(10000 * random_num / CInt(RandomTextBox.Text))
    '					GoTo r_a_2
    '				End If
    '			Else
    '				Process_Int = CInt(10000 * (t - CInt(BurninBox.Text)) / (CInt(TreeBox.Text) - CInt(BurninBox.Text)))
    '			End If
    '			'进度条]

    '		Next

    '		If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
    '		Else
    '			rt_1.Close()
    '		End If
    '		'统计分布地数据！]
    '		Process_Int = 0
    '		Dim write_r As New StreamWriter(root_path + "temp" + path_char + "clade_p.r")
    '		Dim Temp_node_area() As String = particular_node_area.Split(New Char() {"#"c})
    '		write_r.WriteLine(particular_node_num)
    '		Process_Text += " " + vbCrLf
    '		Process_Text += "Result:" + vbCrLf
    '		Process_Text += particular_node_num + vbCrLf

    '		Dim temp_sum As Single = 0
    '		For i As Integer = 1 To UBound(Temp_node_area) - 1 Step 2
    '			If Temp_node_area(i) <> "" Then
    '				temp_sum += CSng(Temp_node_area(i + 1))
    '			End If
    '		Next

    '		If temp_sum > 0 Then
    '			For i As Integer = 1 To UBound(Temp_node_area) - 1 Step 2
    '				If Temp_node_area(i) <> "" Then
    '					write_r.WriteLine(Temp_node_area(i) + "	" + Temp_node_area(i + 1))
    '					Process_Text += Temp_node_area(i) + "	" + Temp_node_area(i + 1) + " (" + (CSng(Temp_node_area(i + 1)) / temp_sum * 100).ToString("F2") + "%)" + vbCrLf
    '				End If
    '			Next
    '			Process_Text += "Relative probability: " + (temp_sum / (CInt(TreeBox.Text) - CInt(BurninBox.Text))).ToString("F4") + vbCrLf
    '		End If

    '		write_r.Close()
    '		show_pie = "clade_p.r"
    '		Enable_Windows()
    '	End Sub
    Public Sub do_analysis()
        'Try

        Me.Invoke(RT1, New Object() {"Analysis begin at " + Date.Now.ToString + vbCrLf})
        Dim r_num As Integer
        Dim use_r_file As Boolean = True
        '[检查文件完整性
        If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
            use_r_file = True
            Dim check_random_r As New StreamReader(root_path + "temp" + path_char + "random_num.txt")
            r_num = CInt(check_random_r.ReadLine)
            Me.Invoke(RTB, New Object() {r_num.ToString})
            For i As Integer = 1 To r_num
                Dim rand_line As String = check_random_r.ReadLine
                If File.Exists(root_path + "temp" + path_char + CInt(rand_line).ToString + ".diva") = False Then
                    Me.Invoke(RT1, New Object() {"Missing DIVA results of tree " + rand_line + "! Cannot do the analysis!"})
                    check_random_r.Close()
                    Enable_Windows()
                    Exit Sub
                End If
            Next
            check_random_r.Close()
        Else
            For t As Integer = CInt(BurninBox.Text) + 1 To CInt(TreeBox.Text)
                Process_Int = CInt(10000 * (t - CInt(BurninBox.Text)) / (CInt(TreeBox.Text) - CInt(BurninBox.Text)))
                If File.Exists(root_path + "temp" + path_char + t.ToString + ".diva") = False Then
                    Me.Invoke(RT1, New Object() {"Missing DIVA results of tree " + t.ToString + "! Cannot do the analysis!"})
                    Enable_Windows()
                    Exit Sub
                End If
            Next
        End If
        '检查文件完整性]

        '[建立最终树数组
        Process_Int = 0

        Dim line As String = ""
        Dim Temp_final_tree As String = final_tree.Replace("(", "#").Replace(")", "#").Replace(",", "#")
        Do
            Temp_final_tree = Temp_final_tree.Replace("##", "#")
        Loop Until Temp_final_tree.Length = Temp_final_tree.Replace("##", "#").Length

        Dim final_tree_arr() As String = Temp_final_tree.Split(New Char() {"#"c})

        Dim final_tree_clade_arr() As String
        Dim final_tree_area_arr() As String
        Dim final_tree_area_arr_sum() As String
        Dim final_tree_area_arr_0() As String
        ReDim final_tree_clade_arr(dtView.Count - 2)
        ReDim final_tree_area_arr(dtView.Count - 2)
        ReDim final_tree_area_arr_sum(dtView.Count - 2)
        ReDim final_tree_area_arr_0(dtView.Count - 2)
        ReDim node_probability(dtView.Count - 2)
        Dim node_line_sdiva() As String
        ReDim node_line_sdiva(dtView.Count - 2)
        If DIVAForm.CheckBox6.Checked = False Then
            Read_Poly_Node(final_tree.Replace(";", ""))
            Dim temp_area As String = ""
            For Each i As String In RangeStr.ToUpper
                temp_area += i + "#0#"
            Next
            For i As Integer = 1 To DIVAForm.ListBox1.Items.Count
                temp_area += DIVAForm.ListBox1.Items(i - 1).ToString + "#0#"
            Next
            temp_area = (temp_area + "#").Replace("##", "")
            For n As Integer = 0 To (final_tree.Length - final_tree.Replace(")", "").Length)
                If Poly_Node(n, 3) <> "" Then
                    Dim Temparry() As String = Poly_Node(n, 3).Split(",")
                    Array.Sort(Temparry)
                    For Each Tempstr As String In Temparry
                        If Tempstr <> "" Then
                            final_tree_clade_arr(n) = final_tree_clade_arr(n) + "#" + Tempstr
                        End If

                    Next
                    final_tree_area_arr(n) = temp_area
                    final_tree_area_arr_sum(n) = temp_area
                    final_tree_area_arr_0(n) = temp_area
                End If

            Next
        Else
            Dim r_f_t As New StreamReader(root_path + "temp" + path_char + "final.diva")

            Do
                If line.StartsWith("optimal distributions") Then
                    Dim node As Integer = 0
                    Dim l As Integer = 0
                    Do
                        line = r_f_t.ReadLine
                        If line <> "" Then
                            If line.StartsWith("node") Then
                                node_line_sdiva(l) = line
                                l = l + 1
                            Else
                                node_line_sdiva(l - 1) = node_line_sdiva(l - 1) + line.Replace("     ", "  ")
                            End If
                        End If

                    Loop Until line = ""
                    For n As Integer = 0 To dtView.Count - 2
                        final_tree_clade_arr(node) = ""
                        Dim cladeline() As String
                        cladeline = node_line_sdiva(n).Substring(node_line_sdiva(n).IndexOf("terminals ") + "terminals ".Length, node_line_sdiva(n).IndexOf(")") - node_line_sdiva(n).IndexOf("terminals ") - "terminals ".Length).Split(New Char() {"-"c})
                        Dim Temparry() As String
                        If Array.IndexOf(final_tree_arr, cladeline(0)) > Array.IndexOf(final_tree_arr, cladeline(1)) Then

                            ReDim Temparry(Array.IndexOf(final_tree_arr, cladeline(0)) - Array.IndexOf(final_tree_arr, cladeline(1)))
                            For i As Integer = Array.IndexOf(final_tree_arr, cladeline(1)) To Array.IndexOf(final_tree_arr, cladeline(0))
                                Temparry(i - Array.IndexOf(final_tree_arr, cladeline(1))) = final_tree_arr(i)
                            Next
                        Else
                            ReDim Temparry(Array.IndexOf(final_tree_arr, cladeline(1)) - Array.IndexOf(final_tree_arr, cladeline(0)))
                            For i As Integer = Array.IndexOf(final_tree_arr, cladeline(0)) To Array.IndexOf(final_tree_arr, cladeline(1))
                                Temparry(i - Array.IndexOf(final_tree_arr, cladeline(0))) = final_tree_arr(i)
                            Next
                        End If
                        Array.Sort(Temparry)
                        For Each Tempstr As String In Temparry
                            final_tree_clade_arr(node) = final_tree_clade_arr(node) + "#" + Tempstr
                        Next
                        '[去除不需要的节点
                        If node_line_sdiva(n).Split(":")(1).Length < 2 Then
                            Dim msg_text As String
                            If DIVAForm.CheckBox1.Checked Then
                                msg_text = "Error: Too much ranges are excluded. Please include more ranges"
                            Else
                                msg_text = "Error: Too much ranges are excluded. Please enable 'Allow Extinction' or include more ranges"
                            End If
                            Process_Text += vbCrLf + msg_text + vbCrLf
                            Process_Int = 0
                            r_f_t.Close()
                            '打印结果]
                            Enable_Windows()
                            Exit Sub
                        End If
                        final_tree_area_arr(node) = "#" + node_line_sdiva(n).Substring(node_line_sdiva(n).IndexOf(":") + 2, node_line_sdiva(n).Length - node_line_sdiva(n).IndexOf(":") - 2).Replace("  ", " ").Replace(" ", "##").Replace("##", "#0#") + "#0"
                        final_tree_area_arr(node) = ("^" + final_tree_area_arr(node)).Replace("^#", "").Replace("^", "")
                        '去除不需要的节点]
                        final_tree_area_arr_sum(node) = final_tree_area_arr(node)
                        final_tree_area_arr_0(node) = final_tree_area_arr(node)
                        node = node + 1
                    Next
                End If
                line = r_f_t.ReadLine
            Loop Until line Is Nothing

            r_f_t.Close()
        End If



        '建立最终树数组]
        '[统计分布地数据！
        '[跳过burnin
        Dim current_tree As String
        Dim burn_in As Integer = CInt(BurninBox.Text)
        Dim has_printrecs As Boolean = False
        Dim has_area As Boolean = False
        Dim sum_area As Integer = 0

        Dim rt_1 As New StreamReader(root_path + B_Tree_File)
        burn_in = CInt(BurninBox.Text)
        Do While burn_in > 0
            rt_1.ReadLine()
            burn_in = burn_in - 1
        Loop
        If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
            rt_1.Close()
        End If

        '跳过burnin]

        '循环处理
        Dim seed As Integer = DateTime.Now.Millisecond
        If Global_seed <> "20180127" Then
            seed = Global_seed
        End If
        Dim rand As New System.Random(seed)
        Dim random_num As Integer = 0
        Dim r_num_a As Integer = 0
        For t As Integer = CInt(BurninBox.Text) + 1 To CInt(TreeBox.Text)
            If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
                If use_r_file Then
                    Dim check_random_r As New StreamReader(root_path + "temp" + path_char + "random_num.txt")
                    r_num_a = check_random_r.ReadLine()
r_a_3:              If r_num_a > 0 Then
                        t = CInt(check_random_r.ReadLine())
                        r_num_a = r_num_a - 1
                        GoTo n_r_a_1
                    End If
                    check_random_r.Close()
                    Exit For
                Else
r_a_2:              If random_num < CInt(RandomTextBox.Text) Then
                        random_num = random_num + 1
                        GoTo r_a_1
                    End If
                    Exit For
                End If

            End If
            GoTo n_r_a_1

r_a_1:      t = rand.Next(CInt(BurninBox.Text) + 1, CInt(TreeBox.Text) + 1)
n_r_a_1:    Dim Totle_length() As Integer
            ReDim Totle_length(dtView.Count - 2)
            For i As Integer = 0 To dtView.Count - 2
                final_tree_area_arr_sum(i) = final_tree_area_arr_0(i)
                Totle_length(i) = 0
            Next


            If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
                Dim random_rt As New StreamReader(root_path + B_Tree_File)
                For i As Integer = 1 To t - 1
                    random_rt.ReadLine()
                Next
                current_tree = random_rt.ReadLine()
                random_rt.Close()
            Else
                current_tree = rt_1.ReadLine()
            End If

            Dim tree_Arr() As String
            tree_Arr = current_tree.Replace("(", "#").Replace(")", "#").Replace(",", "#").Split(New Char() {"#"c})

            Dim r_c_t As New StreamReader(root_path + "temp" + path_char + t.ToString + ".diva")
            line = r_c_t.ReadLine
            Dim Temp_p_n() As Integer
            ReDim Temp_p_n(dtView.Count - 2)
            Do
                'If line.ToUpper.StartsWith("optimal reconstruction".ToUpper) Then

                If line.ToUpper.StartsWith(P_mode.ToUpper) Then
                    has_printrecs = True
                    Dim l As Integer = 0
                    Do
                        line = r_c_t.ReadLine
                        If line <> "" Then
                            If line.StartsWith("node") Then
                                node_line_sdiva(l) = line
                                l = l + 1
                            Else
                                node_line_sdiva(l - 1) = node_line_sdiva(l - 1) + line.Replace("     ", "  ")
                            End If
                        End If
                    Loop Until line = ""
                    For n As Integer = 0 To dtView.Count - 2
                        If node_line_sdiva(n).Split(":")(1).Replace(" ", "") = "" Then
                            Exit Do
                        End If
                    Next
                    For n As Integer = 0 To dtView.Count - 2
                        Dim cladeline() As String
                        cladeline = node_line_sdiva(n).Substring(node_line_sdiva(n).IndexOf("terminals ") + "terminals ".Length, node_line_sdiva(n).IndexOf(")") - node_line_sdiva(n).IndexOf("terminals ") - "terminals ".Length).Split(New Char() {"-"c})
                        Dim Temparry() As String
                        If Array.IndexOf(tree_Arr, cladeline(0)) > Array.IndexOf(tree_Arr, cladeline(1)) Then

                            ReDim Temparry(Array.IndexOf(tree_Arr, cladeline(0)) - Array.IndexOf(tree_Arr, cladeline(1)))
                            For i As Integer = Array.IndexOf(tree_Arr, cladeline(1)) To Array.IndexOf(tree_Arr, cladeline(0))
                                Temparry(i - Array.IndexOf(tree_Arr, cladeline(1))) = tree_Arr(i)
                            Next
                        Else
                            ReDim Temparry(Array.IndexOf(tree_Arr, cladeline(1)) - Array.IndexOf(tree_Arr, cladeline(0)))
                            For i As Integer = Array.IndexOf(tree_Arr, cladeline(0)) To Array.IndexOf(tree_Arr, cladeline(1))
                                Temparry(i - Array.IndexOf(tree_Arr, cladeline(0))) = tree_Arr(i)
                            Next
                        End If
                        Array.Sort(Temparry)
                        Dim Temp_clade As String = ""
                        For Each Tempstr As String In Temparry
                            If Tempstr <> "" Then
                                Temp_clade = Temp_clade + "#" + Tempstr
                            End If
                        Next
                        Dim Temp_index As Integer = Array.IndexOf(final_tree_clade_arr, Temp_clade)
                        If Temp_index >= 0 Then
                            'MsgBox(Temp_clade)

                            Temp_p_n(Temp_index) = 1
                            Dim Temp_area() As String = node_line_sdiva(n).Substring(node_line_sdiva(n).IndexOf(":") + 2, node_line_sdiva(n).Length - node_line_sdiva(n).IndexOf(":") - 2).Replace("  ", " ").Replace(" ", "#").Split(New Char() {"#"c})

                            Dim Temp_final_area() As String
                            Dim add_area As String = ""
                            Temp_final_area = final_tree_area_arr_sum(Temp_index).Split(New Char() {"#"c})
                            For Each i As String In Temp_area
                                If Array.IndexOf(Temp_final_area, i) >= 0 Then
                                    Temp_final_area(Array.IndexOf(Temp_final_area, i) + 1) = (CSng(Temp_final_area(Array.IndexOf(Temp_final_area, i) + 1)) + 1).ToString
                                    has_area = True
                                ElseIf DIVAForm.CheckBox12.Checked = False Then
                                    If DIVAForm.ListBox1.Items.IndexOf(i) >= 0 Then
                                        add_area = "#" + i + "#1"
                                        has_area = True
                                    End If
                                End If
                            Next
                            Totle_length(Temp_index) = Totle_length(Temp_index) + Temp_area.Length
                            '[统计树的分布
                            If has_area Then
                                sum_area = sum_area + 1
                                has_area = False
                            End If
                            '统计树的分布]
                            final_tree_area_arr_sum(Temp_index) = ""
                            For Each i As String In Temp_final_area
                                final_tree_area_arr_sum(Temp_index) = final_tree_area_arr_sum(Temp_index) + i + "#"
                            Next
                            final_tree_area_arr_sum(Temp_index) = (final_tree_area_arr_sum(Temp_index) + "#").Replace("##", "")
                            If add_area <> "" Then
                                final_tree_area_arr_sum(Temp_index) = ("^" + final_tree_area_arr_sum(Temp_index) + add_area).Replace("^#", "").Replace("^", "")
                                final_tree_area_arr(Temp_index) = ("^" + final_tree_area_arr(Temp_index) + add_area.Replace("1", "0")).Replace("^#", "").Replace("^", "")
                                final_tree_area_arr_0(Temp_index) = ("^" + final_tree_area_arr_0(Temp_index) + add_area.Replace("1", "0")).Replace("^#", "").Replace("^", "")
                            End If
                            'MsgBox(final_tree_area_arr(Array.IndexOf(final_tree_clade_arr, Temp_clade)))
                        End If

                    Next

                End If
                line = r_c_t.ReadLine
            Loop Until line Is Nothing
            r_c_t.Close()
            '[累加结果
            For i As Integer = 0 To dtView.Count - 2
                If Totle_length(i) > 0 Then
                    Dim Temp_arry() As String = final_tree_area_arr_sum(i).Split(New Char() {"#"c})
                    Dim Temp_arry1() As String = final_tree_area_arr(i).Split(New Char() {"#"c})
                    For j As Integer = 1 To Temp_arry.Length - 1 Step 2
                        Temp_arry1(j) = CSng(Temp_arry1(j)) + (CSng(Temp_arry(j)) / CSng(Totle_length(i))).ToString
                    Next
                    final_tree_area_arr(i) = ""
                    For Each k As String In Temp_arry1
                        final_tree_area_arr(i) = final_tree_area_arr(i) + k + "#"
                    Next
                    final_tree_area_arr(i) = (final_tree_area_arr(i) + "#").Replace("##", "")
                End If
                node_probability(i) = node_probability(i) + Temp_p_n(i)
            Next
            '累加结果]
            '[进度条
            If CheckBox3.Checked And CInt(RandomTextBox.Text) Then
                If use_r_file Then
                    Process_Int = CInt(10000 * (r_num - r_num_a) / r_num)
                    GoTo r_a_3
                Else
                    Process_Int = CInt(10000 * random_num / CInt(RandomTextBox.Text))
                    GoTo r_a_2
                End If
            Else
                Process_Int = CInt(10000 * (t - CInt(BurninBox.Text)) / (CInt(TreeBox.Text) - CInt(BurninBox.Text)))
            End If
            '进度条]
        Next

        For i As Integer = 0 To dtView.Count - 2
            If CheckBox3.Checked Then
                node_probability(i) = node_probability(i) / CInt(RandomTextBox.Text)
            Else
                node_probability(i) = node_probability(i) / (CInt(TreeBox.Text) - CInt(BurninBox.Text))
            End If
        Next

        If Not (CheckBox3.Checked And CInt(RandomTextBox.Text) > 0) Then
            rt_1.Close()
        End If
        '统计分布地数据！]
        Process_ID = 2
        '[打印结果
        System.Threading.Thread.Sleep(100)
        Begin_Show = False
        System.Threading.Thread.Sleep(100)
        If has_printrecs = False And printrecs Then
            Process_Text += " " + vbCrLf + vbCrLf
            Process_Text += "DIV result do not contain printrecs!" + vbCrLf
            Process_Text += "No available result!" + vbCrLf
            Enable_Windows()
            Exit Sub
        End If
        Process_Text += " " + vbCrLf
        Process_Text += "Result:" + vbCrLf

        Dim w_f_t As New StreamWriter(root_path + "temp" + path_char + "analysis_result.log", False)

        w_f_t.WriteLine("Statistical Dispersal-Vicariance Analysis result file  of " + state_header)


        w_f_t.WriteLine("[TAXON]")
        For i As Integer = 1 To dtView.Count
            w_f_t.WriteLine(dtView.Item(i - 1).Item(0).ToString + "	" + dtView.Item(i - 1).Item(1).ToString + "	" + dtView.Item(i - 1).Item(state_index).ToString.ToUpper)
        Next
        w_f_t.WriteLine("[TREE]")
        w_f_t.WriteLine("Tree=" + tree_show_with_value)
        w_f_t.WriteLine("[RESULT]")
        Dim max_s_value As Single = 0
        If DIVAForm.CheckBox6.Checked Then
            Dim reconstruction_max As String = ""
            Dim reconstruction_num As Integer = 0
            Dim r_f_t_p As New StreamReader(root_path + "temp" + path_char + "final.diva")
            line = r_f_t_p.ReadLine
            Do
                If line.StartsWith("optimal distributions") Then
                    w_f_t.WriteLine(line)
                    Process_Text += line + vbCrLf
                    Dim node As Integer = 0
                    line = r_f_t_p.ReadLine
                    Do
                        If line.StartsWith("node") Then
                            Dim Temp_node() As String = final_tree_area_arr(node).Split(New Char() {"#"c})
                            Dim Temp_node_p() As Single
                            Dim Temp_node_s() As String
                            Dim node_sum As Single = 0
                            ReDim Temp_node_p(CInt((UBound(Temp_node) - 1) / 2))
                            ReDim Temp_node_s(CInt((UBound(Temp_node) - 1) / 2))
                            If Temp_node.Length > 1 Then
                                For i As Integer = 1 To Temp_node.Length Step 2
                                    node_sum = Temp_node(i) + node_sum
                                Next

                                For i As Integer = 1 To Temp_node.Length Step 2
                                    If node_sum <> 0 Then
                                        Temp_node_p((i - 1) / 2) = CSng(Temp_node(i)) / node_sum * 100
                                    Else
                                        Temp_node_p((i - 1) / 2) = 1 / Temp_node.Length * 200
                                    End If
                                    Temp_node_s((i - 1) / 2) = Temp_node(i - 1)
                                Next
                            End If
                            Array.Sort(Temp_node_p, Temp_node_s, New scomparer)
                            Dim Temp_node_line_sdiva As String = ""
                            For i As Integer = 0 To Temp_node_s.Length - 1
                                Temp_node_line_sdiva = Temp_node_line_sdiva + " " + Temp_node_s(i) + " "
                                Temp_node_line_sdiva = Temp_node_line_sdiva + Temp_node_p(i).ToString("F2")
                            Next

                            Process_Text += line.Substring(0, line.IndexOf(":")) + " (P=" + CSng(node_probability(node)).ToString("F2") + "):" + Temp_node_line_sdiva + vbCrLf

                            w_f_t.WriteLine(line.Substring(0, line.IndexOf(":")) + " (P=" + CSng(node_probability(node)).ToString("F2") + "):" + Temp_node_line_sdiva)
                            node = node + 1
                        End If
                        line = r_f_t_p.ReadLine
                    Loop Until line = ""
                End If
                If line <> "" Then
                    If line.StartsWith("optimal reconstruction") And printrecs Then
                        reconstruction_num = reconstruction_num + 1
                        Process_Text += line
                        Dim l As Integer = 0
                        Do
                            line = r_f_t_p.ReadLine
                            If line <> "" Then
                                If line.StartsWith("node") Then
                                    node_line_sdiva(l) = line
                                    l = l + 1
                                Else
                                    node_line_sdiva(l - 1) = node_line_sdiva(l - 1) + line.Replace("     ", "  ")
                                End If
                            End If
                        Loop Until line = ""
                        Dim Temp_node_value As Single = 0
                        Dim Fry() As String
                        ReDim Fry(dtView.Count - 2)
                        For n As Integer = 0 To dtView.Count - 2
                            Dim Temp_area() As String = node_line_sdiva(n).Substring(node_line_sdiva(n).IndexOf(":") + 2, node_line_sdiva(n).Length - node_line_sdiva(n).IndexOf(":") - 2).Replace("  ", " ").Replace(" ", "#").Split(New Char() {"#"c})
                            Dim Temp_final_area() As String = final_tree_area_arr(n).Split(New Char() {"#"c})
                            Dim Temp_length As Single = 0
                            If Temp_final_area.Length > 1 Then
                                For i As Integer = 1 To Temp_final_area.Length Step 2
                                    Temp_length = Temp_length + CSng(Temp_final_area(i))
                                Next
                            End If
                            For i As Integer = 0 To Temp_area.Length - 1
                                If Temp_length > 0 Then
                                    If Array.IndexOf(Temp_final_area, Temp_area(i)) >= 0 Then
                                        Fry(n) = (CSng(Temp_final_area(Array.IndexOf(Temp_final_area, Temp_area(i)) + 1)) / Temp_length * 100).ToString
                                        Temp_node_value = Temp_node_value + (CSng(Temp_final_area(Array.IndexOf(Temp_final_area, Temp_area(i)) + 1)) / Temp_length * 100 * CSng(node_probability(n))).ToString
                                    End If
                                End If
                            Next
                        Next
                        w_f_t.WriteLine("reconstruction " + reconstruction_num.ToString + ", S-DIVA Value=" + Temp_node_value.ToString("F4"))
                        Process_Text += "	S-DIVA Value=	" + Temp_node_value.ToString("F4") + vbCrLf
                        If max_s_value < CSng(Temp_node_value) Then
                            max_s_value = CSng(Temp_node_value)
                            reconstruction_max = reconstruction_num.ToString
                        ElseIf max_s_value = CSng(Temp_node_value) Then
                            reconstruction_max = reconstruction_max + ", " + reconstruction_num.ToString
                        End If
                        For n As Integer = 0 To dtView.Count - 2
                            w_f_t.WriteLine(node_line_sdiva(n) + " " + CSng(Fry(n)).ToString("F2"))
                        Next
                    End If
                End If

                line = r_f_t_p.ReadLine
                'w_f_t.WriteLine(line)
            Loop Until line Is Nothing


            If max_s_value > 0 Then
                Process_Text += vbCrLf + "The maximal S-DIVA Value is " + max_s_value.ToString + " (" + reconstruction_max + ")" + vbCrLf
            End If
            r_f_t_p.Close()
        Else
            w_f_t.WriteLine("Optimal reconstruction:")
            For n As Integer = 0 To UBound(final_tree_area_arr)
                If final_tree_area_arr(n) <> "" Then
                    Dim Temp_node() As String = final_tree_area_arr(n).Split(New Char() {"#"c})
                    Dim Temp_node_p() As Single
                    Dim Temp_node_s() As String
                    Dim node_sum As Single = 0
                    ReDim Temp_node_p(CInt((UBound(Temp_node) - 1) / 2))
                    ReDim Temp_node_s(CInt((UBound(Temp_node) - 1) / 2))
                    If Temp_node.Length > 1 Then
                        For i As Integer = 1 To Temp_node.Length Step 2
                            node_sum = Temp_node(i) + node_sum
                        Next

                        For i As Integer = 1 To Temp_node.Length Step 2
                            If node_sum <> 0 Then
                                Temp_node_p((i - 1) / 2) = CSng(Temp_node(i)) / node_sum * 100
                            Else
                                Temp_node_p((i - 1) / 2) = 1 / Temp_node.Length * 200
                            End If
                            Temp_node_s((i - 1) / 2) = Temp_node(i - 1)
                        Next
                    End If
                    Array.Sort(Temp_node_p, Temp_node_s, New scomparer)
                    Dim Temp_node_line_sdiva As String = ""
                    For i As Integer = 0 To Temp_node_s.Length - 1
                        Temp_node_line_sdiva = Temp_node_line_sdiva + " " + Temp_node_s(i) + " "
                        Temp_node_line_sdiva = Temp_node_line_sdiva + Temp_node_p(i).ToString("F2")
                    Next

                    Process_Text += "node " + (n + 1 + dtView.Count).ToString + ":" + Temp_node_line_sdiva + vbCrLf
                    w_f_t.WriteLine("node " + (n + 1 + dtView.Count).ToString + ":" + Temp_node_line_sdiva)
                End If
            Next

        End If
        w_f_t.Close()
        Process_Int = 0
        '打印结果]
        Enable_Windows()
    End Sub

    Public Function make_final_trees() As Integer
        Dim has_length As Boolean
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf make_final_trees))
        Else
            If File.Exists(root_path + "temp" + path_char + "analysis_result.log") Then
                File.Delete(root_path + "temp" + path_char + "analysis_result.log")
            End If
            tree_show_with_value = final_tree
            Dim rt As StreamReader
            Try
                Dim line As String = ""
                rt = New StreamReader(tree_path)
                line = rt.ReadLine
                Dim f_t_name(,) As String
                ReDim f_t_name(dtView.Count, 1)
                Dim Temp_pv As Integer = 0
                Do While line Is Nothing = False

                    Do
                        If line.StartsWith("	") Or line.StartsWith(" ") Then
                            line = line.Remove(0, 1)
                        Else
                            Exit Do
                        End If
                    Loop

                    If line.Replace("	", "").ToUpper.StartsWith("TRANSLATE") Then
                        line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                        Dim name_num As Integer = 0
                        Do
                            Do
                                If line.StartsWith("	") Or line.StartsWith(" ") Then
                                    line = line.Remove(0, 1)
                                Else
                                    Exit Do
                                End If
                            Loop
                            Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                            f_t_name(name_num, 0) = TRANSLATE(0)
                            f_t_name(name_num, 1) = TRANSLATE(1).Replace("'", "")
                            name_num = name_num + 1
                            line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                        Loop Until line.Contains(";")
                        If line.Replace("	", "").Replace(" ", "").Length > 1 Then
                            Do
                                If line.StartsWith("	") Or line.StartsWith(" ") Then
                                    line = line.Remove(0, 1)
                                Else
                                    Exit Do
                                End If
                            Loop
                            Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                            f_t_name(name_num, 0) = TRANSLATE(0)
                            f_t_name(name_num, 1) = TRANSLATE(1).Replace("'", "")
                            name_num = name_num + 1
                            line = rt.ReadLine.Replace("	", " ")
                        End If
                    End If
                    Process_Int = 4000
                    If line.Replace("	", "").Replace(" ", "").ToUpper.StartsWith("TREE") Or line.Replace("	", "").Replace(" ", "").ToUpper.StartsWith("(") Then
                        Do While line.Contains(";") = False
                            Dim next_tree_line As String = rt.ReadLine
                            If next_tree_line <> "" Then
                                line = line + next_tree_line
                            End If
                        Loop
                        Dim tree_Temp1 As String = line.Substring(line.IndexOf("("), line.Length - line.IndexOf("(")).Replace(" ", "")
                        Dim tree_Temp As String = ""
                        Dim tree_complete As String = ""
                        Dim is_sym As Boolean = False


                        Dim Temp_line As String = ""
                        For Each tree_chr As Char In tree_Temp1
                            If tree_chr = "[" Then
                                is_sym = True
                            End If
                            If tree_chr = "]" Then
                                is_sym = False
                                If Temp_line.IndexOf("posterior=") >= 0 Then
                                    Temp_line = Temp_line.Remove(0, Temp_line.IndexOf("posterior="))
                                    If Temp_line.Contains(",") Then
                                        Temp_line = Temp_line.Substring(Temp_line.IndexOf("=") + 1, Temp_line.IndexOf(",") - Temp_line.IndexOf("=") - 1)
                                    Else
                                        Temp_line = Temp_line.Replace("posterior=", "")
                                    End If
                                    tree_Temp = tree_Temp + Val(Temp_line).ToString("F4")
                                End If

                            End If
                            If is_sym Then
                                Temp_line = Temp_line + tree_chr
                            End If
                            If is_sym = False And tree_chr <> "]" Then
                                tree_Temp = tree_Temp + tree_chr.ToString
                            End If

                        Next
                        Process_Int = 7000

                        If tree_Temp.IndexOf(":") > 0 Then
                            has_length = True
                        End If


                        For Each tree_chr As Char In tree_Temp
                            If tree_chr = ":" Then
                                is_sym = has_length Xor True
                            End If
                            If tree_chr = "," Or tree_chr = "(" Or tree_chr = ")" Then
                                is_sym = False
                            End If
                            If is_sym = False Then
                                tree_complete = tree_complete + tree_chr.ToString
                            End If
                        Next
                        Process_Int = 9000
                        If tree_complete.Replace("(", "").Length - tree_complete.Replace(",", "").Length = 1 And mrbayes_tree Then
                            Dim tree_poly() As Char = tree_complete
                            tree_complete = ""
                            Dim l_c As Integer = 0
                            Dim dh As Integer = 0
                            Dim adddh As Boolean = True
                            For i As Integer = 0 To tree_poly.Length - 1

                                If tree_poly(i) = "(" Then
                                    l_c = l_c + 1
                                End If
                                If tree_poly(i) = "," Then
                                    dh = dh + 1
                                End If
                                If dh = l_c + 1 And adddh Then
                                    If has_length Then
                                        tree_complete = "(" + tree_complete + ")1:0,"
                                    Else
                                        tree_complete = "(" + tree_complete + ")1,"
                                    End If
                                    i = i + 1
                                    adddh = False
                                End If
                                tree_complete = tree_complete + tree_poly(i)
                            Next
                        End If
                        '定义外类群]
                        tree_complete = tree_complete.Replace("'", "").Replace("""", "")
                        For i As Integer = 1 To dtView.Count
                            If f_t_name(i - 1, 0) <> "" And f_t_name(i - 1, 1) <> "" Then
                                tree_complete = tree_complete.Replace("(" + f_t_name(i - 1, 0) + ",", "($%*" + f_t_name(i - 1, 1) + "$%*,")
                                tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ")", ",$%*" + f_t_name(i - 1, 1) + "$%*)")
                                tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ",", ",$%*" + f_t_name(i - 1, 1) + "$%*,")
                                tree_complete = tree_complete.Replace("(" + f_t_name(i - 1, 0) + ":", "($%*" + f_t_name(i - 1, 1) + "$%*:")
                                tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ":", ",$%*" + f_t_name(i - 1, 1) + "$%*:")
                            End If
                        Next
                        Dim wt As New StreamWriter(root_path + "temp" + path_char + "v_tree1.tre", False)
                        wt.WriteLine(tree_complete.Replace("$%*", ""))
                        wt.Close()
                        If has_length Then
                            For i As Integer = 1 To dtView.Count
                                tree_complete = tree_complete.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ":", "($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:")
                                tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ":", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:")
                            Next
                        End If
                        For i As Integer = 1 To dtView.Count
                            tree_complete = tree_complete.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ",", "($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,")
                            tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ")", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*)")
                            tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ",", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,")
                        Next
                        If has_length Then
                            For i As Integer = 1 To dtView.Count
                                tree_complete = tree_complete.Replace("($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:", "(" + dtView.Item(i - 1).Item(0).ToString + ":")
                                tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:", "," + dtView.Item(i - 1).Item(0).ToString + ":")
                            Next
                        End If
                        For i As Integer = 1 To dtView.Count
                            tree_complete = tree_complete.Replace("($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,", "(" + dtView.Item(i - 1).Item(0).ToString + ",")
                            tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*)", "," + dtView.Item(i - 1).Item(0).ToString + ")")
                            tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,", "," + dtView.Item(i - 1).Item(0).ToString + ",")
                        Next
                        tree_complete = tree_complete.Replace(" ", "")
                        tree_show_with_value = tree_complete
                        Me.Invoke(TB4, New Object() {tree_show_with_value})
                        Dim wt1 As New StreamWriter(root_path + "temp" + path_char + "v_tree2.tre", False)
                        wt1.WriteLine(tree_complete)
                        wt1.Close()
                        Process_Int = 10000
                    End If
                    line = rt.ReadLine()
                Loop
                Process_Int = 0
                rt.Close()
                File.Delete(tree_path)
                Enable_Buttun2()
                StartTreeView = True
                Return 0
            Catch ex As Exception
                'Me.Invoke(RT1, New Object() {"Cannot load node frequency, but feel free to continue your work!" + vbCrLf})
                tree_show_with_value = final_tree
                rt.Close()
                Process_Int = 0
                Process_ID = -1
                Enable_Buttun2()
                Enable_Windows()
                Return 1
            End Try
        End If
    End Function
    Public Function load_final_trees() As Integer
        Dim rt As StreamReader
        Try
            File.Copy(tree_path, root_path + "temp" + path_char + "T_Tre.tre", True)

            Dim CopyFileInfo As New FileInfo(root_path + "temp" + path_char + "T_Tre.tre")
            CopyFileInfo.Attributes = FileAttributes.Normal

            tree_path = root_path + "temp" + path_char + "T_Tre.tre"
            Dim line As String = ""
            final_tree = ""
            rt = New StreamReader(tree_path)
            line = rt.ReadLine
            Dim f_t_name(,) As String
            ReDim f_t_name(dtView.Count - 1, 1)
            Do While line Is Nothing = False

                Do
                    If line.StartsWith("	") Or line.StartsWith(" ") Then
                        line = line.Remove(0, 1)
                    Else
                        Exit Do
                    End If
                Loop


                If line.Replace("	", "").Replace(" ", "").ToUpper.StartsWith("TRANSLATE") Then
                    line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                    Dim name_num As Integer = 0
                    Do

                        If line.Length > 0 Then
                            Do
                                If line.StartsWith("	") Or line.StartsWith(" ") Then
                                    line = line.Remove(0, 1)
                                Else
                                    Exit Do
                                End If
                            Loop
                            Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                            f_t_name(name_num, 0) = TRANSLATE(0)
                            f_t_name(name_num, 1) = TRANSLATE(1).Replace("'", "")
                            name_num = name_num + 1
                        End If
                        line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                    Loop Until line.Contains(";")
                    If line.Replace("	", "").Replace(" ", "").Length > 1 Then
                        Do
                            If line.StartsWith("	") Or line.StartsWith(" ") Then
                                line = line.Remove(0, 1)
                            Else
                                Exit Do
                            End If
                        Loop
                        Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                        f_t_name(name_num, 0) = TRANSLATE(0)
                        f_t_name(name_num, 1) = TRANSLATE(1).Replace("'", "")
                        name_num = name_num + 1
                        line = rt.ReadLine.Replace("	", " ")
                    End If
                End If

                If line.Replace("	", "").Replace(" ", "").ToUpper.StartsWith("TREE") Or (line.Replace("	", "").Replace(" ", "").StartsWith("(") And line.Replace("	", "").Replace(" ", "").EndsWith(";")) Then
                    Do While line.Contains(";") = False
                        Dim next_tree_line As String = rt.ReadLine
                        If next_tree_line <> "" Then
                            line = line + next_tree_line
                        End If
                    Loop
                    Dim tree_Temp As String = line.Substring(line.IndexOf("("), line.Length - line.IndexOf("("))
                    Dim tree_Temp1 As String = ""
                    Dim tree_complete As String = ""
                    Dim is_sym As Boolean = False
                    For Each tree_chr As Char In tree_Temp
                        If tree_chr = "[" Then
                            is_sym = True
                        End If
                        If tree_chr = "]" Then
                            is_sym = False
                        End If
                        If is_sym = False And tree_chr <> "]" Then
                            tree_Temp1 = tree_Temp1 + tree_chr.ToString
                        End If
                    Next
                    Process_Int = 500
                    tree_Temp = tree_Temp1
                    tree_Temp1 = ""
                    For Each tree_chr As Char In tree_Temp
                        If tree_chr = ")" Then
                            is_sym = True
                            tree_Temp1 = tree_Temp1 + tree_chr.ToString
                            tree_chr = ""
                        End If
                        If tree_chr = "," Or tree_chr = "(" Or tree_chr = ")" Or tree_chr = ";" Then
                            is_sym = False
                        End If
                        If is_sym = False Then
                            tree_Temp1 = tree_Temp1 + tree_chr.ToString
                        End If
                    Next
                    Process_Int = 1000
                    For Each tree_chr As Char In tree_Temp1
                        If tree_chr = ":" Then
                            is_sym = True
                        End If
                        If tree_chr = "," Or tree_chr = "(" Or tree_chr = ")" Then
                            is_sym = False
                        End If
                        If is_sym = False And tree_chr.ToString <> " " Then
                            tree_complete = tree_complete + tree_chr.ToString
                        End If
                    Next
                    Process_Int = 2000
                    tree_complete = tree_complete.Replace("""", "").Replace("'", "")
                    For i As Integer = 1 To dtView.Count
                        If f_t_name(i - 1, 0) <> "" And f_t_name(i - 1, 1) <> "" Then
                            tree_complete = tree_complete.Replace("(" + f_t_name(i - 1, 0) + ",", "($%*" + f_t_name(i - 1, 1) + "$%*,")
                            tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ")", ",$%*" + f_t_name(i - 1, 1) + "$%*)")
                            tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ",", ",$%*" + f_t_name(i - 1, 1) + "$%*,")
                            tree_complete = tree_complete.Replace("(" + f_t_name(i - 1, 0) + ":", "($%*" + f_t_name(i - 1, 1) + "$%*:")
                            tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ":", ",$%*" + f_t_name(i - 1, 1) + "$%*:")
                        End If
                    Next
                    For i As Integer = 1 To dtView.Count
                        tree_complete = tree_complete.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ",", "($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,")
                        tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ")", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*)")
                        tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ",", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,")
                        tree_complete = tree_complete.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ":", "($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:")
                        tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ":", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:")
                    Next
                    For i As Integer = 1 To dtView.Count
                        tree_complete = tree_complete.Replace("($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,", "(" + dtView.Item(i - 1).Item(0).ToString + ",")
                        tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*)", "," + dtView.Item(i - 1).Item(0).ToString + ")")
                        tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,", "," + dtView.Item(i - 1).Item(0).ToString + ",")
                        tree_complete = tree_complete.Replace("($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:", "(" + dtView.Item(i - 1).Item(0).ToString + ":")
                        tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:", "," + dtView.Item(i - 1).Item(0).ToString + ":")
                    Next
                    tree_complete = tree_complete.Replace(" ", "")
                    If tree_complete.Replace("(", "").Length <> tree_complete.Replace(")", "").Length Then
                        MsgBox("Error 10. missing parentheses in tree! Please check you tree file!")
                        Try
                            rt.Close()
                            Enable_Windows()
                            Enable_Buttun()
                        Catch ex As Exception

                        End Try
                        Return 1
                    End If
                    Process_Int = 3000
                    final_tree = tree_complete
                End If
                line = rt.ReadLine()
            Loop


            Dim outgroup_str As String = ""
            If final_tree.Replace("(", "").Length - final_tree.Replace(",", "").Length = 1 And mrbayes_tree Then
                Dim tree_poly() As Char = final_tree
                final_tree = ""
                Dim l_c As Integer = 0
                Dim dh As Integer = 0
                Dim adddh As Boolean = True
                For i As Integer = 0 To tree_poly.Length - 1

                    If tree_poly(i) = "(" Then
                        l_c = l_c + 1
                    End If
                    If tree_poly(i) = "," Then
                        dh = dh + 1
                    End If
                    If dh = l_c + 1 And adddh Then
                        final_tree = "(" + final_tree + "),"
                        i = i + 1
                        adddh = False
                    End If
                    If adddh = False Then
                        outgroup_str = outgroup_str + tree_poly(i)
                    End If
                    final_tree = final_tree + tree_poly(i)
                Next
                outgroup_str = outgroup_str.Replace("(", "").Replace(")", "").Replace(";", "")
            End If

            If final_tree <> "" Then
                Enable_Buttun1()
                disable_Buttun()
            Else
                Me.Invoke(RT1, New Object() {"Cannot format the tree 1!" + vbCrLf})
                Exit Function
            End If
            rt.Close()

            Dim check_tree() As String
            check_tree = final_tree.Replace(";", "").Replace(",", "|").Replace("(", "|").Replace(")", "|").Replace("||", "|").Split(New Char() {"|"c})
            For Each Tempstr As String In check_tree
                If IsNumeric(Tempstr) = False And Tempstr <> "" Then
                    MsgBox("Cannot find " + Tempstr + " in your trees file!")
                    Process_Int = 0
                    Process_ID = -1
                    Enable_Windows()
                    Exit Function
                End If
            Next

        Catch ex As Exception
            rt.Close()
            MsgBox(ex.ToString)
            Process_Int = 0
            Process_ID = -1
            Enable_Windows()
            MsgBox("Error 2! Cannot format the tree!")
            Return 1
        End Try
        Try
            make_final_trees()
            Fill_Node()
        Catch ex As Exception
            Process_Int = 0
            Process_ID = -1
            Enable_Windows()
            MsgBox("Error 7! Wrong condensed tree!")
            Return 1
        End Try
        Process_Int = 0
        Process_ID = -1
        Enable_Windows()
        Me.Invoke(RT1, New Object() {"Load Condensed Tree Successfully!" + vbCrLf})
        Return 0
    End Function
    Public Function Fill_Node() As Integer
        'Try
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf Fill_Node))
        Else
            If read_node(final_tree.Remove(final_tree.Length - 1, 1), dtView.Count) = -1 Then
                Exit Function
            End If
            get_tree_length(tree_show_with_value.Remove(tree_show_with_value.Length - 1, 1))
            DIVAForm.ComboBox1.Items.Clear()
            For n As Integer = 1 To taxon_num
                DIVAForm.ComboBox1.Items.Add("taxon |" + n.ToString)
            Next
            For n As Integer = 0 To final_tree.Length - final_tree.Replace("(", "").Length - 1
                DIVAForm.ComboBox1.Items.Add("node " + (taxon_num + n + 1).ToString + "|" + Poly_Node(n, 3))
            Next

            Node_Dataset.Tables("NodeTable").Rows.Clear()
            nodeView.AllowNew = True
            Dim temp_Item_Array As Integer = 0
            For n As Integer = 0 To final_tree.Length - final_tree.Replace("(", "").Length - 1
                If (temp_Item_Array = 0 And dtView.Count = 1) = False Then
                    nodeView.AddNew()
                End If
                Dim newrow(1) As String
                newrow(0) = (taxon_num + nodeView.Count).ToString + ":" + Poly_Node(n, 6)
                newrow(1) = Poly_Node(n, 3)
                nodeView.Item(temp_Item_Array).Row.ItemArray = newrow
                temp_Item_Array += 1
            Next
            nodeView.AllowNew = False
        End If
        Return 0
        'Catch ex As Exception
        '    MsgBox(ex.ToString)
        '    Return 1
        'End Try

    End Function

    Private Sub LoadDistrutionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadDistrutionToolStripMenuItem.Click


        DataGridView1.EndEdit()
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "CSV File (*.csv)|*.csv;*.CSV|Phylip File (*.txt)|*.txt;*.TXT|ALL Files(*.*)|*.*"
        opendialog.FileName = ""
        opendialog.Multiselect = False
        opendialog.DefaultExt = ".csv"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            Dim muti_state As Boolean = False
            For i As Integer = Taxon_Dataset.Tables("Taxon Table").Columns.Count - 1 To 2 Step -1
                Taxon_Dataset.Tables("Taxon Table").Columns.RemoveAt(i)
            Next
            Dim dr As New StreamReader(opendialog.FileName)
            Try
                Dim line As String
                Dim temp_d() As String
                Dim temp_t() As String
                Dim temp_state_name() As String
                ReDim temp_d(dtView.Count)
                ReDim temp_t(dtView.Count)
                Dim err As Boolean = False

                If opendialog.FileName.ToLower.EndsWith(".txt") Then
                    line = dr.ReadLine
                    For i As Integer = 1 To dtView.Count
                        line = dr.ReadLine
                        If line <> "" Then
                            temp_t(i) = line.Split(New Char() {"	"c})(0)
                            temp_d(i) = binary_to_dis(line.Split(New Char() {"	"c})(1))
                        End If
                    Next
                    Dim state_count As Integer = 1
                    ReDim temp_state_name(state_count - 1)
                    For i As Integer = 1 To state_count
                        temp_state_name(i - 1) = "State_" + i.ToString
                    Next
                    For i As Integer = 1 To state_count
                        Try
                            Dim Column_Distrution As New System.Data.DataColumn(temp_state_name(i - 1))
                            Taxon_Dataset.Tables("Taxon Table").Columns.Add(Column_Distrution)

                        Catch ex As Exception
                            MsgBox("Name " + temp_state_name(i - 1) + " is repeated. Do not load the column.")
                        End Try
                    Next
                    For i As Integer = 1 To dtView.Count
                        Dim temp_i As Integer = -1
                        temp_i = Array.IndexOf(temp_t, dtView.Item(i - 1).Item(1).ToString)
                        If temp_i > 0 Then
                            dtView.Item(i - 1).Item(2) = temp_d(temp_i)
                        Else
                            err = True
                            Me.Invoke(RT1, New Object() {"Could not find '" + dtView.Item(i - 1).Item(1).ToString + "' in state file! Please examine it before analysis!" + vbCrLf})
                        End If
                    Next
                    For i As Integer = 2 To DataGridView1.Columns.Count - 1
                        DataGridView1.Columns(i).SortMode = DataGridViewColumnSortMode.NotSortable
                    Next
                    state_index = 2
                    state_header = DataGridView1.Columns(state_index).HeaderText
                    set_column_style(3)
                    orgView = dtView
                Else
                    line = dr.ReadLine
                    Dim state_count As Integer = line.Split(New Char() {","c}).Length - 2
                    ReDim temp_state_name(state_count - 1)

                    If IsNumeric(line.Split(New Char() {","c})(0)) Then
                        For i As Integer = 1 To state_count
                            temp_state_name(i - 1) = "State_" + i.ToString
                        Next
                    Else
                        For i As Integer = 1 To state_count
                            temp_state_name(i - 1) = line.Split(New Char() {","c})(i + 1)
                        Next
                        line = dr.ReadLine
                    End If


                    For i As Integer = 1 To dtView.Count
                        If line <> "" Then
                            temp_t(i) = line.Split(New Char() {","c})(1)
                            For j As Integer = 2 To state_count + 1
                                temp_d(i) += "," + line.Split(New Char() {","c})(j).Replace(" ", "").Replace("	", "")
                            Next
                        Else
                            Exit For
                        End If
                        line = dr.ReadLine
                    Next

                    For i As Integer = 1 To state_count
                        Try
                            Dim Column_Distrution As New System.Data.DataColumn(temp_state_name(i - 1))
                            Taxon_Dataset.Tables("Taxon Table").Columns.Add(Column_Distrution)

                        Catch ex As Exception
                            MsgBox("Name " + temp_state_name(i - 1) + " is repeated. Do not load the column.")
                        End Try
                    Next



                    For i As Integer = 1 To dtView.Count
                        Dim temp_i As Integer = -1
                        temp_i = Array.IndexOf(temp_t, dtView.Item(i - 1).Item(1).ToString)
                        If temp_i > 0 Then
                            For j As Integer = 2 To state_count + 1
                                dtView.Item(i - 1).Item(j) = temp_d(temp_i).Split(",")(j - 1)
                            Next
                        Else
                            err = True
                            Me.Invoke(RT1, New Object() {"Could not find '" + dtView.Item(i - 1).Item(1).ToString + "' in state file! Please examine it before analysis!" + vbCrLf})
                        End If
                    Next
                    For i As Integer = 2 To DataGridView1.Columns.Count - 1
                        DataGridView1.Columns(i).SortMode = DataGridViewColumnSortMode.NotSortable
                    Next
                    state_index = 2
                    state_header = DataGridView1.Columns(state_index).HeaderText
                    set_column_style(3)
                    orgView = dtView
                End If

                If err Then
                    MsgBox("Please check your state before analysis!")
                End If
                CmdBox.AppendText("Load States Successfully!" + vbCrLf)

                set_style("PLEASE CHECK THE STATUS BEFORE ANALYSIS", 4)
                DataGridView1.Refresh()
            Catch ex As Exception
                dr.Close()
                MsgBox("Please check your state before analysis!")
            End Try
            dr.Close()
        End If

    End Sub
    Private Sub TreeDataSetToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TreeDataSetToolStripMenuItem.Click
        If File.Exists(root_path + "temp" + path_char + "clean_num_p.trees") Then
            Dim opendialog As New SaveFileDialog
            opendialog.Filter = "Tree File (*.trees)|*.trees;*.TREES|ALL Files(*.*)|*.*"
            opendialog.FileName = ""
            opendialog.DefaultExt = ".trees"
            opendialog.CheckFileExists = False
            opendialog.CheckPathExists = True
            Dim resultdialog As DialogResult = opendialog.ShowDialog()
            If resultdialog = DialogResult.OK Then
                export_omitted(opendialog.FileName, root_path + "temp" + path_char + "clean_num_p.trees")
                Me.Invoke(RT1, New Object() {"Save Successfully!" + vbCrLf})
            End If
        Else
            MsgBox("RASP is formating trees. Please try again later")
        End If

    End Sub
    Public Sub export_omitted(ByVal export_file_name As String, ByVal source_file_name As String)
        Dim wt As New StreamWriter(export_file_name, False)
        wt.WriteLine("#NEXUS")
        wt.WriteLine("")
        wt.WriteLine("Begin taxa;")
        wt.WriteLine("	Dimensions ntax=" + dtView.Count.ToString + ";")
        wt.WriteLine("	Taxlabels")
        For i As Integer = 0 To dtView.Count - 1
            wt.WriteLine("		" + dtView.Item(i)(1))
        Next

        wt.WriteLine("		;")
        wt.WriteLine("End;")
        wt.WriteLine("")
        wt.WriteLine("Begin trees;")
        wt.WriteLine("	Translate")
        For i As Integer = 0 To dtView.Count - 1
            wt.WriteLine("		" + dtView.Item(i)(0).ToString + " " + dtView.Item(i)(1) + ",")
        Next
        wt.WriteLine(";")
        Dim rt As New StreamReader(source_file_name)
        Dim line As String
        Dim o_num As Integer = 0
        Dim tree_num As Integer = 0
        For i As Integer = 1 To CInt(BurninBox.Text)
            line = rt.ReadLine
            o_num = o_num + 1
            Me.Invoke(RT2_S, New Object() {"Processing tree " + o_num.ToString})

        Next
        line = rt.ReadLine
        Do
            wt.WriteLine("tree ID_" + tree_num.ToString + " = " + line)
            line = rt.ReadLine
            tree_num = tree_num + 1
            Me.Invoke(RT2_S, New Object() {"Processing tree " + tree_num.ToString})
        Loop Until line Is Nothing
        wt.WriteLine("End;")
        rt.Close()
        wt.Close()
        MsgBox("Export Successfully! You could load it into RASP as new Tree dataset!")
    End Sub

    Private Sub CleanTempToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        DeleteDir(root_path + "temp")
        Me.Invoke(RT1, New Object() {"Clean Successfully!" + vbCrLf})
    End Sub
    Private Sub PiePictureToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PiePictureToolStripMenuItem.Click
        Dim pie_form As New View_Pie
        pie_form.Show()
    End Sub
    Private Sub LoadConsensusTreeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadConsensusTreeToolStripMenuItem.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Trees (*.tre;*.tree;*.con)|*.tre;*.tree;*.con;|ALL Files(*.*)|*.*"
        opendialog.FileName = ""
        opendialog.DefaultExt = ".tre"
        opendialog.Multiselect = False
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            tree_path = opendialog.FileName
            Disable_Windows()
            Process_Int = 0
            Process_ID = 1
            ProgressBar1.Maximum = 10000
            mrbayes_tree = check_mrbayes()
            Dim make_Tree As New Thread(AddressOf load_final_trees)
            make_Tree.CurrentCulture = ci
            make_Tree.Start()
            set_style("PLEASE CHECK THE STATUS BEFORE ANALYSIS", 4)
        End If
    End Sub
    Public Sub LoadConsensusTree(ByVal tree_url As String)
        tree_path = tree_url
        Disable_Windows()
        Process_Int = 0
        Process_ID = 1
        ProgressBar1.Maximum = 10000
        mrbayes_tree = False
        Dim make_Tree As New Thread(AddressOf load_final_trees)
        make_Tree.CurrentCulture = ci
        make_Tree.Start()
        set_style("PLEASE CHECK THE STATUS BEFORE ANALYSIS", 4)
    End Sub
    Public Function check_mrbayes() As Boolean
        Dim rt As StreamReader
        Try
            Dim line As String = ""
            final_tree = ""
            rt = New StreamReader(tree_path)
            line = rt.ReadLine
            Dim f_t_name(,) As String
            ReDim f_t_name(dtView.Count - 1, 1)
            Do While line Is Nothing = False

                Do
                    If line.StartsWith("	") Or line.StartsWith(" ") Then
                        line = line.Remove(0, 1)
                    Else
                        Exit Do
                    End If
                Loop


                If line.Replace("	", "").Replace(" ", "").ToUpper.StartsWith("TRANSLATE") Then
                    line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                    Dim name_num As Integer = 0
                    Do

                        If line.Length > 0 Then
                            Do
                                If line.StartsWith("	") Or line.StartsWith(" ") Then
                                    line = line.Remove(0, 1)
                                Else
                                    Exit Do
                                End If
                            Loop
                            Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                            f_t_name(name_num, 0) = TRANSLATE(0)
                            f_t_name(name_num, 1) = TRANSLATE(1).Replace("'", "")
                            name_num = name_num + 1
                        End If
                        line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                    Loop Until line.Contains(";")
                    If line.Replace("	", "").Replace(" ", "").Length > 1 Then
                        Do
                            If line.StartsWith("	") Or line.StartsWith(" ") Then
                                line = line.Remove(0, 1)
                            Else
                                Exit Do
                            End If
                        Loop
                        Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                        f_t_name(name_num, 0) = TRANSLATE(0)
                        f_t_name(name_num, 1) = TRANSLATE(1).Replace("'", "")
                        name_num = name_num + 1
                        line = rt.ReadLine.Replace("	", " ")
                    End If
                End If

                If line.Replace("	", "").Replace(" ", "").ToUpper.StartsWith("TREE") Or (line.Replace("	", "").Replace(" ", "").StartsWith("(") And line.Replace("	", "").Replace(" ", "").EndsWith(";")) Then
                    Do While line.Contains(";") = False
                        Dim next_tree_line As String = rt.ReadLine
                        If next_tree_line <> "" Then
                            line = line + next_tree_line
                        End If
                    Loop
                    Dim tree_Temp As String = line.Substring(line.IndexOf("("), line.Length - line.IndexOf("("))
                    Dim tree_Temp1 As String = ""
                    Dim tree_complete As String = ""
                    Dim is_sym As Boolean = False
                    For Each tree_chr As Char In tree_Temp
                        If tree_chr = "[" Then
                            is_sym = True
                        End If
                        If tree_chr = "]" Then
                            is_sym = False
                        End If
                        If is_sym = False And tree_chr <> "]" Then
                            tree_Temp1 = tree_Temp1 + tree_chr.ToString
                        End If
                    Next
                    Process_Int = 500
                    tree_Temp = tree_Temp1
                    tree_Temp1 = ""
                    For Each tree_chr As Char In tree_Temp
                        If tree_chr = ")" Then
                            is_sym = True
                            tree_Temp1 = tree_Temp1 + tree_chr.ToString
                            tree_chr = ""
                        End If
                        If tree_chr = "," Or tree_chr = "(" Or tree_chr = ")" Or tree_chr = ";" Then
                            is_sym = False
                        End If
                        If is_sym = False Then
                            tree_Temp1 = tree_Temp1 + tree_chr.ToString
                        End If
                    Next
                    Process_Int = 1000
                    For Each tree_chr As Char In tree_Temp1
                        If tree_chr = ":" Then
                            is_sym = True
                        End If
                        If tree_chr = "," Or tree_chr = "(" Or tree_chr = ")" Then
                            is_sym = False
                        End If
                        If is_sym = False And tree_chr.ToString <> " " Then
                            tree_complete = tree_complete + tree_chr.ToString
                        End If
                    Next
                    Process_Int = 2000

                    For i As Integer = 1 To dtView.Count
                        If f_t_name(i - 1, 0) <> "" And f_t_name(i - 1, 1) <> "" Then
                            tree_complete = tree_complete.Replace("(" + f_t_name(i - 1, 0) + ",", "($%*" + f_t_name(i - 1, 1) + "$%*,")
                            tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ")", ",$%*" + f_t_name(i - 1, 1) + "$%*)")
                            tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ",", ",$%*" + f_t_name(i - 1, 1) + "$%*,")
                            tree_complete = tree_complete.Replace("(" + f_t_name(i - 1, 0) + ":", "($%*" + f_t_name(i - 1, 1) + "$%*:")
                            tree_complete = tree_complete.Replace("," + f_t_name(i - 1, 0) + ":", ",$%*" + f_t_name(i - 1, 1) + "$%*:")
                        End If
                    Next
                    For i As Integer = 1 To dtView.Count
                        tree_complete = tree_complete.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ",", "($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,")
                        tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ")", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*)")
                        tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ",", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,")
                        tree_complete = tree_complete.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ":", "($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:")
                        tree_complete = tree_complete.Replace("," + dtView.Item(i - 1).Item(1).ToString + ":", ",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:")
                    Next
                    For i As Integer = 1 To dtView.Count
                        tree_complete = tree_complete.Replace("($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,", "(" + dtView.Item(i - 1).Item(0).ToString + ",")
                        tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*)", "," + dtView.Item(i - 1).Item(0).ToString + ")")
                        tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*,", "," + dtView.Item(i - 1).Item(0).ToString + ",")
                        tree_complete = tree_complete.Replace("($%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:", "(" + dtView.Item(i - 1).Item(0).ToString + ":")
                        tree_complete = tree_complete.Replace(",$%*" + dtView.Item(i - 1).Item(1).ToString + "$%*:", "," + dtView.Item(i - 1).Item(0).ToString + ":")
                    Next
                    tree_complete = tree_complete.Replace(" ", "")
                    If tree_complete.Replace("(", "").Length <> tree_complete.Replace(")", "").Length Then
                        MsgBox("Error 10. missing parentheses in tree! Please check you tree file!")
                        Try
                            rt.Close()
                            Enable_Windows()
                            Enable_Buttun()
                        Catch ex As Exception

                        End Try
                        Return 1
                    End If
                    Process_Int = 3000
                    final_tree = tree_complete
                End If
                line = rt.ReadLine()
            Loop


            Dim outgroup_str As String = ""
            If final_tree.Replace("(", "").Length - final_tree.Replace(",", "").Length = 1 Then
                Dim msg_reslut As DialogResult = MsgBox("Find one polytomy." + vbCrLf + "Are you using a unrooted tree or mrBayes tree?", MsgBoxStyle.YesNo)
                If msg_reslut = DialogResult.Yes Then
                    rt.Close()
                    Return True
                Else
                    rt.Close()
                    Return False
                End If
            End If

            rt.Close()
            Return False

        Catch ex As Exception
            rt.Close()
            Return False
        End Try
    End Function

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        End
    End Sub
    Private Sub RunAnalysisToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Private Sub CheckBox2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        printrecs = DIVAForm.CheckBox2.Checked
        If DIVAForm.CheckBox2.Checked Then
            If taxon_num > 64 Then
                Dim msg_reslut As DialogResult = MsgBox("Allow reconstruction will need a larger amount of free space on disk and will take a longer time to run." + vbCrLf + "Are you sure to use it?", MsgBoxStyle.YesNo)
                If msg_reslut = Windows.Forms.DialogResult.Yes Then
                    P_mode = "optimal reconstruction"
                Else
                    DIVAForm.CheckBox2.Checked = False
                End If
            Else
                P_mode = "optimal reconstruction"
            End If
        Else
            P_mode = "optimal distributions"
        End If
    End Sub
    Public Sub ReAnalysis()
        Try

            Disable_Windows()
            do_analysis()
        Catch ex As Exception
            MsgBox("Error, please run complete analysis!")
            Enable_Windows()
        End Try
    End Sub
    Private Sub ReAnalysisToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim ReAna As New Thread(AddressOf ReAnalysis)
        ReAna.CurrentCulture = ci
        ReAna.Start()
    End Sub
    Private Sub RandomTextBox_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles RandomTextBox.KeyPress
        If IsNumeric(e.KeyChar) Or e.KeyChar = Chr(Keys.Back) Then
            Return
        End If
        e.Handled = True
    End Sub
    Private Sub CheckBox3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox3.CheckedChanged
        RandomTextBox.ReadOnly = CheckBox3.Checked Xor True
    End Sub
    Private Sub FormatedTreeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FormatedTreeToolStripMenuItem.Click
        If FinalTreeBox.Text <> "" Then
            Dim opendialog As New SaveFileDialog
            opendialog.Filter = "Nexus Tree File (*.tree)|*.tree;*.TREE|Phylip Tree File (*.tre)|*.tre;*.TRE|ALL Files(*.*)|*.*"
            opendialog.FileName = ""
            opendialog.DefaultExt = ".tree"
            opendialog.CheckFileExists = False
            opendialog.CheckPathExists = True
            Dim resultdialog As DialogResult = opendialog.ShowDialog()
            If resultdialog = DialogResult.OK Then
                Dim wr As New StreamWriter(opendialog.FileName, False)
                If opendialog.FileName.ToUpper.EndsWith(".TRE") Then
                    Dim temp_tree As String = FinalTreeBox.Text
                    For i As Integer = 1 To dtView.Count
                        temp_tree = temp_tree.Replace("(" + dtView.Item(i - 1).Item(0).ToString + ",", "($%*" + dtView.Item(i - 1).Item(0).ToString + "$%*,")
                        temp_tree = temp_tree.Replace("," + dtView.Item(i - 1).Item(0).ToString + ")", ",$%*" + dtView.Item(i - 1).Item(0).ToString + "$%*)")
                        temp_tree = temp_tree.Replace("," + dtView.Item(i - 1).Item(0).ToString + ",", ",$%*" + dtView.Item(i - 1).Item(0).ToString + "$%*,")
                        temp_tree = temp_tree.Replace("," + dtView.Item(i - 1).Item(0).ToString + ":", ",$%*" + dtView.Item(i - 1).Item(0).ToString + "$%*:")
                        temp_tree = temp_tree.Replace("(" + dtView.Item(i - 1).Item(0).ToString + ":", "($%*" + dtView.Item(i - 1).Item(0).ToString + "$%*:")
                    Next
                    For i As Integer = 1 To dtView.Count
                        temp_tree = temp_tree.Replace("($%*" + dtView.Item(i - 1).Item(0).ToString + "$%*:", "(" + dtView.Item(i - 1).Item(1).ToString + ":")
                        temp_tree = temp_tree.Replace(",$%*" + dtView.Item(i - 1).Item(0).ToString + "$%*:", "," + dtView.Item(i - 1).Item(1).ToString + ":")
                        temp_tree = temp_tree.Replace("($%*" + dtView.Item(i - 1).Item(0).ToString + "$%*,", "(" + dtView.Item(i - 1).Item(1).ToString + ",")
                        temp_tree = temp_tree.Replace(",$%*" + dtView.Item(i - 1).Item(0).ToString + "$%*)", "," + dtView.Item(i - 1).Item(1).ToString + ")")
                        temp_tree = temp_tree.Replace(",$%*" + dtView.Item(i - 1).Item(0).ToString + "$%*,", "," + dtView.Item(i - 1).Item(1).ToString + ",")
                    Next
                    wr.WriteLine(temp_tree)
                Else
                    wr.WriteLine("#NEXUS")
                    wr.WriteLine("Begin trees;")
                    wr.WriteLine("   Translate")
                    For i As Integer = 1 To dtView.Count - 1
                        wr.WriteLine(dtView.Item(i - 1).Item(0).ToString + " " + dtView.Item(i - 1).Item(1).ToString + ",")
                    Next
                    wr.WriteLine(dtView.Item(dtView.Count - 1).Item(0).ToString + " " + dtView.Item(dtView.Count - 1).Item(1).ToString)
                    wr.WriteLine("		;")
                    wr.WriteLine("tree con = [&R] " + FinalTreeBox.Text)
                    wr.WriteLine("End;")

                End If
                wr.Close()
            End If
        Else
            MsgBox("You have not made a Condensed Tree!")
        End If

    End Sub
    Private Sub TreeBox_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TreeBox.KeyPress
        If IsNumeric(e.KeyChar) Or e.KeyChar = Chr(Keys.Back) Then
            Return
        End If
        e.Handled = True
    End Sub
    Private Sub TreeViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TreeViewToolStripMenuItem.Click
        Dim Tree_view_form As New View_Tree
        Tree_view_form.current_state = state_index
        Tree_view_form.Show()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        Dim aboutform As New Form_AboutBox
        aboutform.Show()
    End Sub
    Dim nulldataset As New DataSet
    Private Sub CloseDataToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseDataToolStripMenuItem.Click
        If isDebug = False Then
            DeleteDir(root_path + "temp")
        End If
        View_Dis.Hide()
        Tool_Combine.Hide()
        Tool_SvT.Hide()
        View_Tracer.Hide()
        View_OptionForm.Hide()
        Config_Lagrange.Hide()
        Config_BGB.Hide()
        Tool_DPP.Hide()
        Config_SDIVA.Hide()
        Config_BBM.Hide()
        View_Tracer.Hide()
        Config_BayArea.Hide()
        Config_Traits.Hide()
        Config_Chrom.Hide()
        close_data()
        DataGridView1.EndEdit()
    End Sub
    Public Sub close_data()
        Process_ID = -1
        state_index = 2
        state_header = ""
        Taxon_Dataset.Tables("Taxon Table").Clear()

        Node_Dataset.Tables("NodeTable").Clear()
        TreeInfo.Text = ""
        For i As Integer = Taxon_Dataset.Tables("Taxon Table").Columns.Count - 1 To 3 Step -1
            Taxon_Dataset.Tables("Taxon Table").Columns.RemoveAt(i)
        Next
        DataGridView1.Columns(3).HeaderText = "State"
        If dtView.Count = 1 Then
            Dim newrow(2) As String
            newrow(0) = ""
            newrow(1) = ""
            newrow(2) = ""
            dtView.Item(0).Row.ItemArray = newrow
        End If

        If nodeView.Count = 1 Then
            Dim newrow(1) As String
            newrow(0) = ""
            newrow(1) = ""
            nodeView.Item(0).Row.ItemArray = newrow
        End If

        DIVAForm.DataGridView2.Columns.Clear()
        DPPdiv_Config.DataGridView5.Columns.Clear()
        TraitsForm.DataGridView2.Columns.Clear()
        BGB_Config.NumericUpDown2.Minimum = 1
        BGB_Config.NumericUpDown2.Value = 2

        Lagrange_Config.NumericUpDown2.Minimum = 1
        Lagrange_Config.NumericUpDown2.Value = 2
        taxon_num = 0
        tree_show = ""
        tree_show_with_value = ""
        particular_node_num = ""
        B_Tree_File = ""
        final_tree_file = ""
        final_tree = ""
        Tree_Num_B = 0
        Tree_Num_P = 0
        BeforeLoad.Text = "0"
        DIVAForm.ComboBox1.Items.Clear()

        taxon_num = 0
        ReDim gene_names(0)

        TreeBox.Text = 0
        TreeBox_P.Text = 0
        BurninBox.Text = 0
        FinalTreeBox.Text = ""
        config_SDIVA_node = ""
        config_SDIVA_omitted = ""
        CmdBox.Text = ""

        DIVAForm.CheckBox10.Checked = False
        DIVAForm.CheckBox11.Checked = False

        LoadDistrutionToolStripMenuItem.Enabled = False
        LoadFinalTreeToolStripMenuItem.Enabled = False
        LoadTreesToolStripMenuItem.Enabled = True
        LoadTreesDataToolStripMenuItem.Enabled = True
        QuickLoadToolStripMenuItem.Enabled = True
        LoadOneTreeToolStripMenuItem.Enabled = True
        AddTreesDataToolStripMenuItem.Enabled = False
        OriginalMethodsToolStripMenuItem.Enabled = False
        StatisticalMethodsToolStripMenuItem.Enabled = False
        TraitsEvolutionToolStripMenuItem.Enabled = False
        ModelTestToolStripMenuItem.Enabled = False
        CloseDataToolStripMenuItem.Enabled = False
        OmittedTaxaToolStripMenuItem1.Enabled = False
        DPPAnalysisToolStripMenuItem.Enabled = False
        LoadDataToolStripMenuItem.Enabled = False
        TracerViewToolStripMenuItem.Enabled = False
        TreeDataSetToolStripMenuItem.Enabled = False
        RandomTreesToolStripMenuItem.Enabled = False
        ComparisonToolStripMenuItem.Enabled = False
        OtherToolStripMenuItem.Enabled = False
    End Sub
    Public Function add_og(ByVal tree_line As String, ByVal og_id As Integer) As String
        Return "(" + tree_line.Remove(tree_line.Length - 1) + "," + og_id.ToString + ");"
    End Function
    Public Sub make_final_tree()
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf make_final_tree))
        Else
            Dim line As String = ""
            Dim Temp As String = ""
            Dim rt As New StreamReader(root_path + "temp" + path_char + "outtree")
            Do
                Temp = rt.ReadLine
                line = line + Temp
            Loop Until Temp Is Nothing
            rt.Close()

            Dim f_t_name() As String
            ReDim f_t_name(dtView.Count)

            Dim tree_Temp1 As String = ""
            Dim tree_Temp As String = line.Replace("):", ")#")
            Dim tree_complete As String = ""
            Dim is_sym As Boolean = False
            Dim value As String = ""
            For Each tree_chr As Char In tree_Temp
                If tree_chr = ":" Then
                    is_sym = True
                End If
                If tree_chr = "," Or tree_chr = "(" Or tree_chr = ")" Then
                    is_sym = False
                End If
                If is_sym = False Then
                    tree_Temp1 = tree_Temp1 + tree_chr.ToString
                End If
            Next

            For Each tree_chr As Char In tree_Temp1
                If tree_chr = "#" Then
                    is_sym = True
                End If
                If tree_chr = "," Or tree_chr = "(" Or tree_chr = ")" Then
                    is_sym = False
                End If
                If is_sym = False Then
                    If value <> "" Then
                        If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
                            tree_complete = tree_complete + (CSng(value) / (CInt(RandomTextBox.Text))).ToString("F2") + tree_chr.ToString
                        Else
                            tree_complete = tree_complete + (CSng(value) / (CInt(TreeBox_P.Text) - CInt(BurninBox.Text))).ToString("F2") + tree_chr.ToString
                        End If


                        value = ""
                    Else

                        tree_complete = tree_complete + tree_chr.ToString
                    End If
                Else
                    If tree_chr <> "#" Then
                        value = value + tree_chr
                    End If
                End If

            Next

            tree_Temp1 = ""
            For Each tree_chr As Char In line
                If tree_chr = ":" Then
                    is_sym = True
                End If
                If tree_chr = "," Or tree_chr = "(" Or tree_chr = ")" Then
                    is_sym = False
                End If
                If is_sym = False Then
                    tree_Temp1 = tree_Temp1 + tree_chr.ToString
                End If
            Next
            final_tree = tree_Temp1
            tree_show_with_value = tree_complete.Replace("$%#", "").Replace(";", "1.00;")
            FinalTreeBox.Text = tree_show_with_value
            If read_node(final_tree.Remove(final_tree.Length - 1, 1), dtView.Count) = -1 Then
                Exit Sub
            End If
            get_tree_length(tree_show_with_value.Remove(tree_show_with_value.Length - 1, 1))
            Fill_Node()
            StartTreeView = True
        End If
    End Sub
    Public Sub Build_Final_Tree()
        Disable_Windows()
        If File.Exists(root_path + "temp" + path_char + "outfile") Then
            File.Delete(root_path + "temp" + path_char + "outfile")
        End If
        If File.Exists(root_path + "temp" + path_char + "outtree") Then
            File.Delete(root_path + "temp" + path_char + "outtree")
        End If
        If File.Exists(root_path + "temp" + path_char + "intree") Then
            File.Delete(root_path + "temp" + path_char + "intree")
        End If
        DataGridView1.EndEdit()
        Dim og As Integer = -1
        Dim dfog As Boolean = False

        For i As Integer = 1 To dtView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                og = i.ToString
            End If
        Next
        If og = -1 Then
            og = dtView.Count + 1
        End If
        Dim firstline As String = ""
        Process_ID = 1
        If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
            ProgressBar1.Maximum = CInt(RandomTextBox.Text)
            Dim seed As Integer = DateTime.Now.Millisecond
            If Global_seed <> "20180127" Then
                seed = Global_seed
            End If
            Dim rand As New System.Random(seed)
            Dim current_tree As String = ""
            Dim Analyse_trees As New StreamWriter(root_path + "temp" + path_char + "intree", False)
            Dim t As Integer = 0
            Me.Invoke(RT2_S, New Object() {"Processing tree ..."})
            t = rand.Next(CInt(BurninBox.Text) + 1, CInt(TreeBox.Text))
            Dim random_rt As New StreamReader(root_path + P_Tree_File)
            For i As Integer = 1 To t - 1
                random_rt.ReadLine()
            Next
            current_tree = random_rt.ReadLine()
            If og = dtView.Count + 1 Then
                current_tree = add_og(current_tree, og)
            End If
            firstline = current_tree
            random_rt.Close()

            Analyse_trees.WriteLine(current_tree)
            For random_num As Integer = 2 To CInt(RandomTextBox.Text)
                t = rand.Next(CInt(BurninBox.Text) + 1, CInt(TreeBox.Text))
                cons_tre = random_num
                Process_Int = random_num
                Dim random_rt1 As New StreamReader(root_path + P_Tree_File)
                For i As Integer = 1 To t - 1
                    random_rt1.ReadLine()
                Next
                current_tree = random_rt1.ReadLine()
                If og = dtView.Count + 1 Then
                    current_tree = add_og(current_tree, og)
                End If
                random_rt1.Close()

                Analyse_trees.WriteLine(current_tree)
            Next
            Analyse_trees.Close()
        Else
            ProgressBar1.Maximum = CInt(TreeBox_P.Text)
            Dim rt As New StreamReader(root_path + P_Tree_File)
            Dim wt As New StreamWriter(root_path + "temp" + path_char + "intree", False)
            Me.Invoke(RT2_S, New Object() {"Jumping tree ..."})
            For i As Integer = 1 To CInt(BurninBox.Text)
                cons_tre = i
                Process_Int = i
                rt.ReadLine()

            Next
            Dim line As String = rt.ReadLine
            If og = dtView.Count + 1 Then
                line = add_og(line, og)
            End If
            firstline = line
            Me.Invoke(RT2_S, New Object() {"Processing tree ..."})
            Dim P_ID As Integer = CInt(BurninBox.Text)
            Do
                P_ID = P_ID + 1
                Process_Int = P_ID

                wt.WriteLine(line)
                line = rt.ReadLine
                If og = dtView.Count + 1 Then
                    line = add_og(line, og)
                End If
            Loop Until line Is Nothing Or P_ID = CInt(TreeBox.Text)
            wt.Close()
            rt.Close()
        End If
        Process_Int = 0
        Process_ID = 3
        Dim f_t_name() As String = firstline.Replace("(", "").Replace(")", "").Replace(";", ",").Split(New Char() {","c})
        Me.Invoke(RT2_S, New Object() {"Consense Tree ..."})
        Try
            og = (Array.IndexOf(f_t_name, og.ToString) + 1)
            cons_tre = 0
            ProgressBar1.Maximum = CInt(TreeBox_P.Text) - CInt(BurninBox.Text)
            Dim temp_i As Integer = og

            current_dir = Directory.GetCurrentDirectory
            Directory.SetCurrentDirectory(root_path)
            ctree(temp_i, root_path, cons_tre)
            Directory.SetCurrentDirectory(current_dir)
            If og = dtView.Count + 1 Then
                Dim line As String = ""
                Dim Temp As String = ""
                Dim rt As New StreamReader(root_path + "temp" + path_char + "outtree")
                Do
                    Temp = rt.ReadLine
                    line = line + Temp
                Loop Until Temp Is Nothing
                rt.Close()
                line = line.Remove(0, 1)
                line = line.Remove(line.LastIndexOf(","))
                line = line.Remove(line.LastIndexOf(")")) + ");"
                Dim sw As New StreamWriter(root_path + "temp" + path_char + "outtree")
                sw.WriteLine(line)
                sw.Close()
            End If

            format_path()
            cons_tre = -1
            Me.Invoke(RT2_S, New Object() {""})
        Catch ex As Exception
            MsgBox(ex.ToString)
            If File.Exists(root_path + "temp" + path_char + "outfile") Then
                File.Delete(root_path + "temp" + path_char + "outfile")
            End If
            If File.Exists(root_path + "temp" + path_char + "outtree") Then
                File.Delete(root_path + "temp" + path_char + "outtree")
            End If
            If File.Exists(root_path + "temp" + path_char + "intree") Then
                File.Delete(root_path + "temp" + path_char + "intree")
            End If
            'PiePictureToolStripMenuItem.Enabled = True
            FormatedTreeToolStripMenuItem.Enabled = True
            RandomTreesToolStripMenuItem.Enabled = True
            TreeDataSetToolStripMenuItem.Enabled = True
            OmittedTaxaToolStripMenuItem1.Enabled = True
            Enable_Windows()
            MsgBox(ex.ToString)
            Exit Sub
        End Try
        Process_ID = -1
        make_final_tree()
        If File.Exists(root_path + "temp" + path_char + "outfile") Then
            File.Delete(root_path + "temp" + path_char + "outfile")
        End If
        If File.Exists(root_path + "temp" + path_char + "outtree") Then
            File.Delete(root_path + "temp" + path_char + "outtree")
        End If
        If File.Exists(root_path + "temp" + path_char + "intree") Then
            File.Delete(root_path + "temp" + path_char + "intree")
        End If
        'PiePictureToolStripMenuItem.Enabled = True
        FormatedTreeToolStripMenuItem.Enabled = True
        RandomTreesToolStripMenuItem.Enabled = True
        TreeDataSetToolStripMenuItem.Enabled = True
        OmittedTaxaToolStripMenuItem1.Enabled = True
        Enable_Windows()
        Enable_Buttun2()
        Me.Invoke(RT1, New Object() {"Consense Completed!" + vbCrLf})
        add_tree_length()
        CheckForIllegalCrossThreadCalls = True
    End Sub
    Private Sub MakeFinalTreeToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MakeFinalTreeToolStripMenuItem1.Click
        StartTreeView = True
        DataGridView1.EndEdit()
        If IsNumeric(BurninBox.Text) And CInt(BurninBox.Text) <= CInt(TreeBox.Text) Then
            If CInt(TreeBox_P.Text) <= 1 Then
                MsgBox("Need at least two tree!")
                Exit Sub
            End If
        Else
            MsgBox("Burn-in error!")
            Exit Sub
        End If
        'Dim og As String = ""
        'For i As Integer = 1 To dtView.Count
        '    If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
        '        og = i.ToString
        '    End If
        'Next

        'If og = "" Then
        '    Dim og_res As DialogResult = MsgBox("You do not select outgroup. All trees will be treated as rooted.", MsgBoxStyle.Information)
        'End If
        'Dim dia_res As DialogResult = MsgBox("Use taxon " + og.ToString + " as outgroup?", MsgBoxStyle.YesNo)
        'If dia_res = Windows.Forms.DialogResult.Yes Then
        current_dir = Directory.GetCurrentDirectory
        Directory.SetCurrentDirectory(root_path)
        cons_tre = 0
        CheckForIllegalCrossThreadCalls = False
        Dim l_Tree As New Thread(AddressOf Build_Final_Tree)
        l_Tree.CurrentCulture = ci
        l_Tree.Start()
        set_style("PLEASE CHECK THE STATUS BEFORE ANALYSIS", 4)
        'End If
    End Sub
    Private Sub OmittedTaxaToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OmittedTaxaToolStripMenuItem1.Click
        If final_tree <> "" Then
            taxon_num = dtView.Count
            tree_show = final_tree
            Dim t_view As New Tool_Omitted
            For i As Integer = 1 To DIVAForm.ComboBox1.Items.Count
                t_view.ComboBox1.Items.Add(DIVAForm.ComboBox1.Items(i - 1).ToString)
            Next
            t_view.TreeBox.Text = TreeBox.Text
            t_view.BurninBox.Text = BurninBox.Text
            t_view.Show()
        Else
            MsgBox("You must have a condensed tree before define omitted groups!")
        End If
    End Sub
    Private Sub Range_Timer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DIVA_Timer.Tick
        If RangeMade Then
            RangeMade = False
            DIVA_Timer.Enabled = False
            current_dir = Directory.GetCurrentDirectory
            Directory.SetCurrentDirectory(root_path)
            Disable_Windows()
            Dim diva As New Thread(AddressOf runDIVA)
            diva.CurrentCulture = ci
            diva.Start()
        End If
    End Sub
    Public file_names() As String
    Public tree_filename As String
    Dim files_complete As Boolean = True
    Private Sub LoadTreesDataToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadTreesDataToolStripMenuItem.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Trees dataset|*.trees;*.TREES;*.trees.txt;*.t;*.T;*.tre;*.TRE;*.tree;*.TREE;*.nex;*.NEX|Tree (*.tre)|*.tre;*.TRE|Mrbayes Tree Data (*.t)|*.t;*.T|ALL Files(*.*)|*.*"
        opendialog.FileName = ""
        opendialog.Multiselect = True
        opendialog.DefaultExt = ".trees"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            If opendialog.FileNames.Length > 1 Then
                tree_path = opendialog.FileNames(0)
            Else
                tree_path = opendialog.FileName
            End If
            Me.Activate()
            Disable_Windows()
            CloseDataToolStripMenuItem.Enabled = True
            tree_path = opendialog.FileName
            If tree_path.ToLower.EndsWith(".t") Then
                mrbayes_tree = True
            Else
                mrbayes_tree = False
            End If

            error_no = load_names(opendialog.FileName.Substring(opendialog.FileName.LastIndexOf(".") + 1))
            If error_no = 0 Then
                Process_Text += "Loading Trees Dataset ... " + vbCrLf
                If File.Exists(root_path + "temp" + path_char + "clean_num.trees") Then
                    File.Delete(root_path + "temp" + path_char + "clean_num.trees")
                End If
                If opendialog.FileNames.Length > 1 Then
                    file_names = opendialog.FileNames
                    files_complete = False
                    Dim l_Tree As New Thread(AddressOf load_muti_trees)
                    l_Tree.CurrentCulture = ci
                    l_Tree.Start()
                Else
                    Dim l_Tree As New Thread(AddressOf load_trees)
                    l_Tree.CurrentCulture = ci
                    l_Tree.Start()
                End If
            End If
            set_style("PLEASE CHECK THE STATUS BEFORE ANALYSIS", 4)
        End If
    End Sub
    Public Sub load_tree_data(ByVal trees_url As String)
        Me.Activate()
        Disable_Windows()
        CloseDataToolStripMenuItem.Enabled = True
        tree_path = trees_url
        If tree_path.ToLower.EndsWith(".t") Then
            mrbayes_tree = True
        Else
            mrbayes_tree = False
        End If
        error_no = load_names(trees_url.Substring(trees_url + 1))
        If error_no = 0 Then
            Process_Text += "Loading Trees Dataset ... " + vbCrLf
            If File.Exists(root_path + "temp" + path_char + "clean_num.trees") Then
                File.Delete(root_path + "temp" + path_char + "clean_num.trees")
            End If
            Dim l_Tree As New Thread(AddressOf load_trees)
            l_Tree.CurrentCulture = ci
            l_Tree.Start()
        End If
        set_style("PLEASE CHECK THE STATUS BEFORE ANALYSIS", 4)
    End Sub
    Public Sub load_muti_trees()
        For i As Integer = 0 To UBound(file_names)
            If i = UBound(file_names) Then
                files_complete = True
            End If
            tree_path = file_names(i)
            load_trees()
        Next
    End Sub
    Private Sub AddTreesDataToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddTreesDataToolStripMenuItem.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Trees (*.trees)|*.trees;*.TREES;*.trees.txt;*.tre;*.TRE;*.t;*.T|Tree (*.tre)|*.tre;*.TRE|Mrbayes Tree Data (*.t)|*.t;*.T|ALL Files(*.*)|*.*"
        opendialog.FileName = ""
        opendialog.Multiselect = True
        opendialog.DefaultExt = ".trees"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            Disable_Windows()
            CloseDataToolStripMenuItem.Enabled = True
            If opendialog.FileNames.Length > 1 Then
                file_names = opendialog.FileNames
                files_complete = False
                Dim l_Tree As New Thread(AddressOf load_muti_trees)
                l_Tree.CurrentCulture = ci
                l_Tree.Start()
            Else
                tree_path = opendialog.FileName
                Dim l_Tree As New Thread(AddressOf load_trees)
                l_Tree.CurrentCulture = ci
                l_Tree.Start()
            End If
            set_style("PLEASE CHECK THE STATUS BEFORE ANALYSIS", 4)
        End If
    End Sub

    Public Sub read_bayarea()
        Dim Treeline As String
        Dim bayareacyc As Integer = Int(config_BayArea_cycle / config_BayArea_fre)
        Treeline = tree_show_with_value.Replace(";", "")
        Dim NumofTaxon As Integer = Treeline.Length - Treeline.Replace(",", "").Length + 1
        Dim NumofNode As Integer = Treeline.Length - Treeline.Replace("(", "").Length
        get_tree_length(Treeline)

        Dim line As String
        Dim result_p() As String
        ReDim result_p(NumofNode - 1)
        Dim p_list(,) As Integer
        ReDim p_list(NumofNode - 1, RangeStr.Length)
        Array.Clear(p_list, 0, NumofNode * (RangeStr.Length + 1))

        Dim sr As New StreamReader(root_path + "temp" + path_char + "bayarea.areas.txt.parameters.txt")
        line = sr.ReadLine
        Dim sample_count As Double
        sample_count = (config_BayArea_cycle - Config_BayArea_Burnin) / config_BayArea_fre
        If Config_BayArea_Burnin > 0 Then
            sample_count = Int(sample_count) + 1
        End If
        sr.Close()
        Dim result_f() As String
        ReDim result_f(NumofNode - 1)
        For n As Integer = 1 To NumofNode
            bayarea_gen = CInt(n * bayareacyc / NumofNode) - 1
            Dim area_str_Arry() As String
            Dim area_str_prob() As Integer
            ReDim area_str_Arry(0)
            ReDim area_str_prob(0)
            Dim temp_node_id As Integer = -1
            Array.Clear(area_str_Arry, 0, area_str_Arry.Length)
            Array.Clear(area_str_prob, 0, area_str_prob.Length)
            Dim sr_area As New StreamReader(root_path + "temp" + path_char + "bayarea.areas.txt.area_states.txt")
            sr_area.ReadLine()
            For i As Integer = 1 To Int(Max((Config_BayArea_Burnin / config_BayArea_fre - 1), 0) * NumofNode)
                sr_area.ReadLine()
            Next
            For i As Integer = 1 To NumofTaxon
                sr_area.ReadLine()
            Next
            Dim current_id As Integer
            For j As Integer = 1 To sample_count
                For i As Integer = 1 To n
                    line = sr_area.ReadLine()
                Next
                Dim temp_array() As String = line.Split("	")
                If temp_node_id = -1 Then
                    temp_node_id = CInt(temp_array(2))
                End If
                current_id = temp_node_id - NumofTaxon
                For i As Integer = 1 To temp_array(3).Length
                    If temp_array(3).Chars(i - 1) = "1" Then
                        p_list(current_id, i) += 1
                    End If
                Next
                Dim temp_index As Integer = Array.IndexOf(area_str_Arry, temp_array(3))
                If temp_index >= 0 Then
                    area_str_prob(temp_index) += 1
                Else
                    ReDim Preserve area_str_Arry(UBound(area_str_Arry) + 1)
                    area_str_Arry(UBound(area_str_Arry)) = temp_array(3)
                    ReDim Preserve area_str_prob(UBound(area_str_prob) + 1)
                    area_str_prob(UBound(area_str_prob)) += 1
                End If
                For i As Integer = n + 1 To NumofNode
                    line = sr_area.ReadLine()
                Next
            Next
            sr_area.Close()
            Array.Sort(area_str_prob, area_str_Arry, New scomparer)
            temp_node_id = Left_to_right(temp_node_id + 1, Treeline)
            Dim t_list() As String = Poly_Node(temp_node_id - NumofTaxon - 1, 3).Split(New Char() {","c})
            result_f(temp_node_id - NumofTaxon - 1) = "node " + temp_node_id.ToString + " (anc. of terminals " + t_list(0) + "-" + t_list(UBound(t_list)) + "):"
            For k As Integer = 0 To UBound(area_str_prob)
                If area_str_Arry(k) <> "" Then
                    result_f(temp_node_id - NumofTaxon - 1) += " " + binary_to_dis(area_str_Arry(k)) + " " + (area_str_prob(k) / sample_count * 100).ToString("F2")
                End If
            Next
            result_p(temp_node_id - NumofTaxon - 1) = "node " + temp_node_id.ToString + ":"
            For i As Integer = 1 To RangeStr.Length
                result_p(temp_node_id - NumofTaxon - 1) += "	" + (1 - p_list(current_id, i) / sample_count).ToString("F6") + "	" + (p_list(current_id, i) / sample_count).ToString("F6")
            Next
            Process_Text += vbCrLf + result_f(temp_node_id - NumofTaxon - 1)
        Next

        Dim swf As StreamWriter
        swf = New StreamWriter(root_path + "temp" + path_char + "analysis_result.log", False)
        swf.WriteLine("Bayarea Analysis Result file of " + state_header)
        swf.WriteLine("[TAXON]")
        For i As Integer = 1 To dtView.Count
            swf.WriteLine(dtView.Item(i - 1).Item(0).ToString + "	" + dtView.Item(i - 1).Item(1).ToString + "	" + dtView.Item(i - 1).Item(state_index).ToString.ToUpper)
        Next
        swf.WriteLine("[TREE]")
        swf.WriteLine("Tree=" + tree_show_with_value)
        swf.WriteLine("[RESULT]")
        swf.WriteLine("Result of Bayarea:")
        For Each i As String In result_f
            swf.WriteLine(i)
        Next
        swf.WriteLine("[PROBABILITY]")
        Dim temp_log As String = "	"
        For i As Integer = 1 To RangeStr.Length
            temp_log = temp_log + Chr(i + 64).ToString + "(0)	" + Chr(i + 64).ToString + "(1)	"
        Next
        swf.WriteLine(temp_log)
        For Each i As String In result_p
            swf.WriteLine(i)
        Next
        swf.WriteLine("[END]")
        swf.Close()

        Process_Text += vbCrLf + "Analysis end at " + Date.Now.ToString + vbCrLf
        tree_view_title = "Bayarea Analysis Result"
        Me.Invoke(RT2_S, New Object() {""})
        StartTreeView = True
        bayarea_gen = -3
        CheckForIllegalCrossThreadCalls = True
    End Sub
    Public Sub read_range_log()
        Me.Invoke(RT2_S, New Object() {"Second setp  ..."})
        Dim waitforbayes As Boolean = False
        Do
            Try
                If File.Exists(root_path + "temp" + path_char + "clade1.nex.mcmc") Then
                    File.Delete(root_path + "temp" + path_char + "clade1.nex.mcmc")
                    waitforbayes = False
                End If
                If File.Exists(root_path + "temp" + path_char + "clade1.nex.run1.t") Then
                    File.Delete(root_path + "temp" + path_char + "clade1.nex.run1.t")
                    waitforbayes = False
                End If
                If File.Exists(root_path + "temp" + path_char + "clade1.nex.run2.t") Then
                    File.Delete(root_path + "temp" + path_char + "clade1.nex.run2.t")
                    waitforbayes = False
                End If
                If File.Exists(root_path + "temp" + path_char + "analysis_result.log") Then
                    File.Delete(root_path + "temp" + path_char + "analysis_result.log")
                    waitforbayes = False
                End If
                waitforbayes = True
            Catch ex As Exception
                System.Threading.Thread.Sleep(512)
            End Try
        Loop Until waitforbayes
        Dim sr As New StreamReader(root_path + "temp" + path_char + "clade_b.log")
        Dim line As String
        line = sr.ReadLine
        Dim swf As StreamWriter

        swf = New StreamWriter(root_path + "temp" + path_char + "analysis_result.log", False)
        swf.WriteLine("Bayesian Analysis result file of " + state_header)
        swf.WriteLine("[TAXON]")
        For i As Integer = 1 To dtView.Count
            swf.WriteLine(dtView.Item(i - 1).Item(0).ToString + "	" + dtView.Item(i - 1).Item(1).ToString + "	" + dtView.Item(i - 1).Item(state_index).ToString.ToUpper)
        Next
        swf.WriteLine("[TREE]")
        swf.WriteLine("Tree=" + tree_show_with_value)
        swf.WriteLine("[RESULT]")
        Dim result_r1() As String
        Dim result_r2() As String
        Dim result_f() As String
        Dim node_number As Integer = tree_show_with_value.Length - tree_show_with_value.Replace("(", "").Length
        ReDim result_r1(node_number - 1)
        ReDim result_r2(node_number - 1)
        ReDim result_f(node_number - 1)
        ReDim num_r1(0)
        ReDim num_r2(0)
        Dim Temp_array As Integer = 0
        Process_Text += vbCrLf + "Result of run 1:" + vbCrLf
        Do
            Dim p_list() As String = line.Split(New Char() {"="c})(1).Split(New Char() {"	"c})
            For Each i As String In p_list
                If i <> "" Then
                    num_r1(UBound(num_r1)) = Val(i)
                    ReDim Preserve num_r1(UBound(num_r1) + 1)
                End If

            Next
            result_r1(Temp_array) = make_range_P(0, p_list, BayesForm.NumericUpDown1.Value, line.Split(New Char() {"="c})(0).Replace(" ", "") + ".r")
            Temp_array += 1
            line = sr.ReadLine
        Loop Until line.StartsWith("-")
        Temp_array = 0
        line = sr.ReadLine
        Process_Text += vbCrLf + "Result of run 2:" + vbCrLf
        Do
            Dim p_list() As String = line.Split(New Char() {"="c})(1).Split(New Char() {"	"c})
            For Each i As String In p_list
                If i <> "" Then
                    num_r2(UBound(num_r2)) = Val(i)
                    ReDim Preserve num_r2(UBound(num_r2) + 1)
                End If

            Next
            result_r2(Temp_array) = make_range_P(0, p_list, BayesForm.NumericUpDown1.Value, line.Split(New Char() {"="c})(0).Replace(" ", "") + ".r")
            Temp_array += 1
            line = sr.ReadLine
        Loop Until line.StartsWith("-")
        Temp_array = 0
        line = sr.ReadLine
        Dim distance As Single = 0
        For i As Integer = 0 To UBound(num_r1) - 1
            distance = distance + (num_r1(i) - num_r2(i)) ^ 2 ^ 0.5 / 2
        Next
        distance = distance / (UBound(num_r1) - 1)
        Process_Text += vbCrLf + "distance of run 1 and run 2: " + distance.ToString("F4") + vbCrLf
        Process_Text += vbCrLf + "Result of combined:" + vbCrLf
        Do
            Dim p_list() As String = line.Split(New Char() {"="c})(1).Split(New Char() {"	"c})
            result_f(Temp_array) = make_range_P(0, p_list, BayesForm.NumericUpDown1.Value, line.Split(New Char() {"="c})(0).Replace(" ", "") + ".b")
            Temp_array += 1
            line = sr.ReadLine
        Loop Until line.StartsWith("-")
        Temp_array = 0
        swf.WriteLine("Result of combined:")

        For Each i As String In result_f
            swf.WriteLine(i)
        Next
        swf.WriteLine("Result of run 1:")

        For Each i As String In result_r1
            swf.WriteLine(i)
        Next
        swf.WriteLine("Result of run 2:")
        For Each i As String In result_r2
            swf.WriteLine(i)
        Next

        swf.WriteLine("[PROBABILITY]")
        sr.Close()
        Dim st As New StreamReader(root_path + "temp" + path_char + "clade_b.log")
        Dim temp_log As String
        Do
            temp_log = st.ReadLine
        Loop Until temp_log.StartsWith("-")
        Do
            temp_log = st.ReadLine
        Loop Until temp_log.StartsWith("-")
        temp_log = "	"
        For i As Integer = 1 To RangeStr.Length
            temp_log = temp_log + Chr(i + 64).ToString + "(0)	" + Chr(i + 64).ToString + "(1)	"
        Next
        swf.WriteLine(temp_log)
        temp_log = st.ReadLine
        Do
            If temp_log.StartsWith("-") = False Then
                Dim cur_clade As Integer = CInt(temp_log.Split(New Char() {" "c})(0).ToLower.Replace("clade", "").Replace("_p", ""))
                Dim result As String = "node " + (cur_clade + taxon_num).ToString
                temp_log = result + ": " + temp_log.Split(New Char() {"="c})(1)
                swf.WriteLine(temp_log)
            End If
            temp_log = st.ReadLine
        Loop Until temp_log = ""
        st.Close()
        swf.WriteLine("[END]")
        swf.Close()
        bayesIsrun = False
        Dim isclear As Boolean = True
        Do
            Try
                isclear = True
                If File.Exists(root_path + "temp" + path_char + "clade1.nex") Then
                    isclear = False
                    File.Delete(root_path + "temp" + path_char + "clade1.nex")
                End If
            Catch ex As Exception
                System.Threading.Thread.Sleep(500)
            End Try
        Loop Until isclear
        Process_Text += vbCrLf + "Analysis end at " + Date.Now.ToString + vbCrLf
        tree_view_title = "Bayesian Binary MCMC Analysis Result"
        Me.Invoke(RT2_S, New Object() {""})
        StartTreeView = True
        Process_ID = -1
    End Sub

    Public Function make_range_P(ByVal model As Integer, ByVal Single_P1() As String, ByVal Max_area As Single, ByVal ID As String) As String
        Dim NewCom As New Module_PerCom
        Dim Temp_range() As String
        Dim ComArray As Object
        Dim range_p(0) As Single
        Dim range_str(0) As String
        Dim rang_array As Integer = 0
        Dim rang_sum As Single = 0
        ReDim Temp_range(RangeStr.Length - 1)
        For i As Integer = 0 To RangeStr.Length - 1
            Temp_range(i) = Chr(65 + i)
        Next
        Select Case model
            Case 0
                For j As Integer = 1 To Max_area
                    ComArray = NewCom.Combine(Temp_range, j)
                    ReDim Preserve range_p(UBound(ComArray) + range_p.Length)
                    ReDim Preserve range_str(UBound(range_p))
                    For i As Integer = 0 To UBound(ComArray)
                        Dim line() As Char = Distributiton_to_Binary(ComArray(i), RangeStr.Length)
                        Dim Temp_p As Single = 1

                        For k As Integer = 0 To UBound(line)
                            If line(k) = "0" Then
                                Temp_p = Temp_p * (Val(Single_P1(k * 2 + 1)))
                            Else
                                Temp_p = Temp_p * (Val(Single_P1(k * 2 + 2)))
                            End If
                        Next
                        rang_sum = rang_sum + Temp_p
                        range_p(rang_array) = Temp_p
                        range_str(rang_array) = ComArray(i)
                        rang_array += 1
                    Next
                Next
            Case 1
                ReDim range_p(1)
                ReDim range_str(1)
                For k As Integer = 0 To RangeStr.Length - 1
                    range_p(0) = 1
                    If Val(Single_P1(k * 2 + 2)) >= 0.5 Then
                        range_p(0) = range_p(0) * (Val(Single_P1(k * 2 + 2)))
                        range_str(0) += ChrW(65 + k)
                    End If
                Next
                rang_sum = 1
        End Select
        If BayesForm.CheckBox1.Checked And model = 0 Then
            ReDim Preserve range_p(range_p.Length)
            ReDim Preserve range_str(UBound(range_p))
            Dim line() As Char = Distributiton_to_Binary("/", RangeStr.Length)
            Dim Temp_p As Single = 1
            For k As Integer = 0 To UBound(line)
                Temp_p = Temp_p * (Val(Single_P1(k * 2 + 1)))
            Next
            rang_sum = rang_sum + Temp_p
            range_p(rang_array) = Temp_p
            range_str(rang_array) = "/"
        End If
        Array.Sort(range_p, range_str, New scomparer)
        Dim cur_clade As Integer = CInt(ID.Split(New Char() {"."c})(0).ToLower.Replace("clade", "").Replace("_p", ""))
        Dim t_list() As String = Poly_Node(cur_clade - 1, 3).Split(New Char() {","c})
        Dim result As String = "node " + (cur_clade + taxon_num).ToString + " (anc. of terminals " + t_list(0) + "-" + t_list(UBound(t_list)) + "):"
        If rang_sum > 0 Then
            Dim sw As New StreamWriter(root_path + "temp" + path_char + ID)
            sw.WriteLine(result)
            Process_Text += "node " + (cur_clade + taxon_num).ToString
            Process_Text += " (terminals " + t_list(0) + "-" + t_list(UBound(t_list)) + "):"
            For i As Integer = 0 To UBound(range_p) - 1
                If i < 3 Or range_p(i) / rang_sum * 100 >= 5 Then
                    Process_Text += " " + range_str(i) + " " + (range_p(i) / rang_sum * 100).ToString("F2") + "%"
                End If
                result = result + " " + range_str(i) + " " + (range_p(i) / rang_sum * 100).ToString("F2")
                sw.WriteLine(range_str(i) + "	" + (range_p(i) / rang_sum * 100).ToString("F2"))
            Next
            Process_Text += " " + vbCrLf
            sw.Close()
        End If
        Return result
    End Function

    Private Sub BayesianBinaryAnalysisToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)


    End Sub
    Private Sub FinalTreeBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FinalTreeBox.TextChanged
        If FinalTreeBox.Text <> "" Then
            Me.Invoke(RT1, New Object() {"Using Tree: " + FinalTreeBox.Text + vbCrLf})
        End If
    End Sub
    Private Sub SDECToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Private Sub BayesTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Bayes_Timer.Tick
        If bayes_gen <> -1 And bayes_gen <> -2 Then
            Process_ID = -2
            If BayesForm.CheckBox2.Checked Then
                If File.Exists(root_path + "temp\endbbm") Then
                    File.Delete(root_path + "temp\endbbm")
                    Bayes_Timer.Enabled = False
                    build_result()
                    bayes_gen = -1
                    Bayes_Timer.Enabled = True
                    ProgressBar1.Value = 0
                Else
                    If File.Exists(root_path + "temp\clade1.nex.mcmc") Then
                        File.Copy(root_path + "temp\clade1.nex.mcmc", root_path + "temp\bbm.count", True)
                        Dim line As String
                        Dim conut_line As Integer = 0
                        Dim start_count As Boolean = False
                        Dim sr As New StreamReader(root_path + "temp\bbm.count")
                        Do
                            line = sr.ReadLine
                            conut_line += 1
                        Loop Until line Is Nothing
                        sr.Close()
                        bayes_gen = Min(Max(conut_line - 7, 0) * 1000, CInt(BayesForm.TextBox3.Text))
                        Me.Invoke(RT2_S, New Object() {"Setp 1: " + bayes_gen.ToString + "/" + BayesForm.TextBox3.Text})
                        Me.Invoke(PV, New Object() {bayes_gen})
                    End If
                End If

            Else
                Try
                    Me.Invoke(RT2_S, New Object() {"Setp 1: " + bayes_gen.ToString + "/" + BayesForm.TextBox3.Text})
                    Me.Invoke(PV, New Object() {bayes_gen})
                Catch ex As Exception

                End Try
            End If
        Else
            Select Case bayes_gen
                Case -1
                    Me.Invoke(PV, New Object() {0})
                    statebox.Text = ""
                    Dim read_log As New Thread(AddressOf read_range_log)
                    read_log.CurrentCulture = ci
                    read_log.Start()
                    Bayes_Timer.Enabled = False
                    Process_ID = -1
                Case -2
                    Me.Invoke(PV, New Object() {0})
                    statebox.Text = ""
                    Dim read_log As New Thread(AddressOf read_new_bayes_log)
                    read_log.CurrentCulture = ci
                    read_log.Start()
                    Bayes_Timer.Enabled = False
                    Process_ID = -1
            End Select

        End If
    End Sub
    Dim rasp_result As String
    Dim node_id() As Integer
    Dim node_value() As String
    Dim node_list() As String
    Private Sub FDTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Main_Timer.Tick
        Select Case Process_ID
            Case 0
                Me.Invoke(TB3, New Object() {Process_Gen})
                Me.Invoke(TB1, New Object() {Process_Gen1})
                TreeBox.Text = Process_Gen1
                TreeBox_P.Text = Process_Gen
            Case 1
                ProgressBar1.Value = Process_Int
            Case 2
                CmdBox.AppendText(Process_Text)
                Process_Text = ""
            Case 3
                If cons_tre <> -1 Then
                    Try
                        Me.Invoke(PV, New Object() {cons_tre})
                    Catch ex As Exception

                    End Try
                Else
                    Me.Invoke(PV, New Object() {0})
                    cons_tre = 0
                End If
            Case 4
                If diva_gen > 0 Then
                    Try
                        Process_Int = Min(10000, 10000 * diva_gen / PV_SUM)
                        ProgressBar1.Value = Process_Int
                    Catch ex As Exception

                    End Try
                Else
                    ProgressBar1.Value = 0
                End If
            Case 5
                FD_6_Combine()
                Process_Text = ""
                Process_Int = 0
                Process_ID = 1
            Case 6
                Dim check_end As Boolean = True
                For i As Integer = 0 To muti_threads_DEC - 1
                    If File.Exists(root_path + "temp\SDEC_" + i.ToString + ".end") = False Then
                        check_end = False
                        Exit For
                    End If
                Next
                If check_end Or File.Exists(root_path + "temp\lg_err.txt") = True Then
                    Dim th_fd_6 As New Thread(AddressOf FD_6_Combine)
                    th_fd_6.Start()
                    Process_Text = ""
                    Process_Int = 0
                    Process_ID = 1
                Else
                    lag_gen = 10000 * Count_Files(root_path + "temp", ".dec") / sdec_count
                    ProgressBar1.Value = Min(10000, lag_gen)
                End If
            Case 7
                Select Case bayarea_gen
                    Case -1
                        bayarea_gen = 0
                        CheckForIllegalCrossThreadCalls = False
                        If BayAreaForm.CheckBox1.Checked Then
                            Dim sr As New StreamReader(root_path + "temp" + path_char + "bayarea.areas.txt.nhx")
                            Dim sw As New StreamWriter(root_path + "temp" + path_char + "bayarea.areas.new.nhx")
                            Dim line As String = ""
                            Do
                                line = sr.ReadLine
                                sw.WriteLine(line)
                            Loop Until line.StartsWith("	Taxlabels")
                            For i As Integer = 0 To dtView.Count - 1
                                line = sr.ReadLine
                                line = "		" + dtView.Item(i).Item(1)
                                sw.WriteLine(line)
                            Next
                            Do
                                line = sr.ReadLine
                                sw.WriteLine(line)
                            Loop Until line.StartsWith("	Translate")
                            For i As Integer = 0 To dtView.Count - 2
                                line = sr.ReadLine
                                line = "		" + i.ToString + "	" + dtView.Item(i).Item(1) + ","
                                sw.WriteLine(line)
                            Next
                            line = sr.ReadLine
                            line = "		" + (dtView.Count - 1).ToString + "	" + dtView.Item(dtView.Count - 1).Item(1)
                            sw.WriteLine(line)
                            line = sr.ReadLine
                            Do
                                sw.WriteLine(line)
                                line = sr.ReadLine
                            Loop Until line Is Nothing
                            sr.Close()
                            sw.Close()
                            File.Copy(root_path + "temp" + path_char + "bayarea.areas.txt.area_probs.txt", (BayAreaForm.TextBox5.Text + "\") + "bayarea.areas.txt.area_probs.txt", True)
                            File.Copy(root_path + "temp" + path_char + "bayarea.areas.txt.parameters.txt", (BayAreaForm.TextBox5.Text + "\") + "bayarea.areas.txt.parameters.txt", True)
                            File.Copy(root_path + "temp" + path_char + "bayarea.areas.txt.area_states.txt", (BayAreaForm.TextBox5.Text + "\") + "bayarea.areas.txt.area_states.txt", True)
                            File.Copy(root_path + "temp" + path_char + "bayarea.areas.new.nhx", (BayAreaForm.TextBox5.Text + "\") + "bayarea.areas.txt.nhx", True)
                        End If
                        Dim th1 As New Thread(AddressOf read_bayarea)
                        th1.Start()
                    Case -2
                        Me.Invoke(RT2_S, New Object() {"Waiting for Burn-in value"})
                        ProgressBar1.Value = 0
                        TracerViewToolStripMenuItem.Enabled = True
                        bayarea_gen = -4
                        ProgressBar1.Value = 0
                        TracerForm.Show()
                    Case -3
                        CmdBox.AppendText(Process_Text)
                        ProgressBar1.Value = 0
                        Me.Invoke(RT1, New Object() {"Open [Graphic->Tree View] to see the result" + vbCrLf})
                        Me.Invoke(RT2_S, New Object() {""})
                        Process_ID = -2
                        bayesIsrun = False
                        Enable_Windows()
                    Case -4
                    Case Else
                        Disable_Windows()
                        CmdBox.AppendText(Process_Text)
                        Process_Text = ""
                        Me.Invoke(RT2_S, New Object() {bayarea_gen.ToString + "/" + (config_BayArea_cycle / config_BayArea_fre).ToString})
                        ProgressBar1.Value = Min(10000, CInt(10000 / config_BayArea_cycle * config_BayArea_fre * bayarea_gen))
                End Select
            Case 8 'remove outgroup
                Process_ID = 1
                CheckForIllegalCrossThreadCalls = False
                Dim th1 As New Thread(AddressOf write_remove_og)
                th1.Start()
            Case 9
                If BGB_gen = -1 Or File.Exists(root_path + "temp\BGB.end") Then
                    Main_Timer.Enabled = False
                    If File.Exists(root_path + "temp\err.log") Then
                        Try
                            Dim sr As New StreamReader(root_path + "temp\err.log")
                            Me.Invoke(RT1, New Object() {"ERROR:" + sr.ReadToEnd + vbCrLf})
                            Me.Invoke(RT1, New Object() {"Try [Tools->Scaling Branch Length] if you meet errors." + vbCrLf})
                            Me.Invoke(RT2_S, New Object() {"BioGenBEARS analysis failed"})

                            Enable_Windows()
                            Process_Text = ""
                            Process_Int = 0
                            Process_ID = -1
                            BGB_gen = -1
                            Main_Timer.Enabled = True
                            sr.Close()
                            Exit Sub
                        Catch ex As Exception
                            Me.Invoke(RT2_S, New Object() {"BioGenBEARS analysis failed"})

                            Enable_Windows()
                            Process_Text = ""
                            Process_Int = 0
                            Process_ID = -1
                            BGB_gen = -1
                            Main_Timer.Enabled = True
                            Exit Sub
                        End Try
                    End If
                    Select Case BGB_mode
                        Case 0
                            read_BGB(root_path + "temp\tabDEC.txt", root_path + "temp\final.tre", "DEC", False)
                            read_BGB(root_path + "temp\tabDECj.txt", root_path + "temp\final.tre", "DEC+j", True)
                            read_BGB(root_path + "temp\tabDIVALIKE.txt", root_path + "temp\final.tre", "DIVALIKE", True)
                            read_BGB(root_path + "temp\tabDIVALIKEj.txt", root_path + "temp\final.tre", "DIVALIKE+j", True)
                            read_BGB(root_path + "temp\tabBAYAREALIKE.txt", root_path + "temp\final.tre", "BAYAREALIKE", True)
                            read_BGB(root_path + "temp\tabBAYAREALIKEj.txt", root_path + "temp\final.tre", "BAYAREALIKE+j", True)
                        Case 2
                            read_BGB(root_path + "temp\tab" + BGB_Config.ComboBox1.Text + ".txt", root_path + "temp\final.tre", BGB_Config.ComboBox1.Text, False)
                        Case 3
                            read_BGB(root_path + "temp\tabDEC.txt", root_path + "temp\final.tre", "DEC", False)
                            read_BGB(root_path + "temp\tabDIVALIKE.txt", root_path + "temp\final.tre", "DIVALIKE", True)
                            read_BGB(root_path + "temp\tabBAYAREALIKE.txt", root_path + "temp\final.tre", "BAYAREALIKE", True)
                    End Select
                    rasp_result += "[SUPPLEMENT]"
                    If File.Exists(root_path + "temp\restable_AICc_rellike_formatted.txt") Then
                        CmdBox.AppendText("# You could simply choose the model with the highest AICc_wt value as the 'Best' model," + vbCrLf)
                        CmdBox.AppendText("# and apply this model on your trees data set." + vbCrLf)
                        Dim sr0 As New StreamReader(root_path + "temp\restable_AICc_rellike_formatted.txt")
                        Process_Text = sr0.ReadToEnd
                        sr0.Close()
                        CmdBox.AppendText("#Results of Model Test#" + vbCrLf)
                        CmdBox.AppendText(Process_Text)
                        CmdBox.AppendText("# Use the AICc to select the best model" + vbCrLf)
                        Process_Text = Process_Text.Replace("LnL	", "	LnL	")
                        rasp_result += vbCrLf + "#Results of Model Test#"
                        rasp_result += vbCrLf + Process_Text
                        rasp_result += vbCrLf + "# Use the highest AICc_wt to select the best model"

                    End If
                    If File.Exists(root_path + "temp\teststable.txt") Then
                        Dim sr1 As New StreamReader(root_path + "temp\teststable.txt")
                        Process_Text = sr1.ReadToEnd
                        Process_Text = Process_Text.Replace("1	DEC+J", "DEC+J")
                        Process_Text = Process_Text.Replace("2	DIVALIKE+J", "DIVALIKE+J")
                        Process_Text = Process_Text.Replace("3	BAYAREALIKE+J", "BAYAREALIKE+J")
                        Process_Text = Process_Text.Replace("LnL	", "	LnL	")

                        sr1.Close()

                        CmdBox.AppendText("# The p-value of the LRT (Likelihood Ratio Test) tells you whether or not you can reject" + vbCrLf)
                        CmdBox.AppendText("# the null hypothesis that without J and +J confer equal likelihoods on the data." + vbCrLf)
                        CmdBox.AppendText(Process_Text)
                        rasp_result += vbCrLf + "==============================================================="
                        rasp_result += vbCrLf + Process_Text
                        rasp_result += vbCrLf + "# The p-value of the LRT (Likelihood Ratio Test) tells you whether or not you can reject"
                        rasp_result += vbCrLf + "# the null hypothesis that without J and +J confer equal likelihoods on the data."

                    End If
                    rasp_result += vbCrLf + "[END]"
                    Me.Invoke(RT1, New Object() {"Process end at " + Date.Now.ToString + vbCrLf})
                    Dim wr As New StreamWriter(root_path + "temp\analysis_result.log")
                    wr.Write(rasp_result)
                    wr.Close()

                    Me.Invoke(RT2_S, New Object() {""})
                    ProgressBar1.Value = 0
                    Me.Invoke(RT1, New Object() {"Open [Graphic->Tree View] to see the result" + vbCrLf})
                    Enable_Windows()
                    Process_Text = ""
                    Process_Int = 0
                    Process_ID = -1
                    StartTreeView = True
                    tree_view_title = "BioGeoBEARS Analysis Result"
                    Main_Timer.Enabled = True
                Else
                    Me.Invoke(RT2_S, New Object() {"BioGenBEARS package is running ..."})
                    If File.Exists(root_path + "temp\BGB.state") Then
                        Try
                            Dim sr As New StreamReader(root_path + "temp\BGB.state")
                            BGB_gen = CInt(sr.ReadLine)
                            sr.Close()
                            ProgressBar1.Value = BGB_gen
                        Catch ex As Exception

                        End Try
                    Else
                        Me.Invoke(RT2_S, New Object() {"BioGeoBEARS package is loading ..."})
                    End If
                End If
            Case 10
                Dim check_end As Boolean = True
                For i As Integer = 0 To muti_threads_BGB - 1
                    If File.Exists(root_path + "temp\SBGB_" + i.ToString + ".end") = False Then
                        check_end = False
                        Exit For
                    End If
                Next
                If BGB_gen = -1 Or check_end Then
                    If File.Exists(root_path + "temp\err.log") Then
                        Try
                            Dim sr As New StreamReader(root_path + "temp\err.log")
                            Me.Invoke(RT1, New Object() {"ERROR:" + sr.ReadToEnd + vbCrLf})
                            Me.Invoke(RT1, New Object() {"Try [Tools->Scaling Branch Length] if you meet errors about branch length <=0" + vbCrLf})
                            Me.Invoke(RT2_S, New Object() {"BioGenBEARS analysis failed"})
                            Enable_Windows()
                            Process_Text = ""
                            Process_Int = 0
                            Process_ID = -1
                            BGB_gen = -1
                            Main_Timer.Enabled = True
                            sr.Close()
                            Exit Sub
                        Catch ex As Exception
                            Me.Invoke(RT2_S, New Object() {"BioGenBEARS analysis failed"})
                            Enable_Windows()
                            Process_Text = ""
                            Process_Int = 0
                            Process_ID = -1
                            BGB_gen = -1
                            Main_Timer.Enabled = True
                            Exit Sub
                        End Try
                    End If

                    Dim th_fd_10 As New Thread(AddressOf FD_10_Combine)
                    th_fd_10.Start()
                    Process_Text = ""
                    Process_Int = 0
                    Process_ID = -2
                    ProgressBar1.Value = 0
                Else
                    Me.Invoke(RT2_S, New Object() {"BioGeoBEARS package is running ..."})
                    Try
                        If File.Exists(root_path + "temp\BGB.state") Then
                            BGB_gen = Min(10000, 10000 * Count_Files(root_path + "temp", ".tab") / SBGB_count)
                            ProgressBar1.Value = BGB_gen
                        Else
                            Me.Invoke(RT2_S, New Object() {"BioGeoBEARS package is loading ..."})
                        End If
                    Catch ex As Exception

                    End Try

                End If
            Case 11
                Dim check_end As Boolean = True
                For i As Integer = 0 To muti_threads_DIVA - 1
                    If File.Exists(root_path + "temp\DIVA_" + i.ToString + ".end") = False Then
                        check_end = False
                        Exit For
                    End If
                Next
                If check_end Then
                    Dim th_fd_11 As New Thread(AddressOf FD_11_Combine)
                    th_fd_11.Start()
                    Process_Text = ""
                    Process_Int = 0
                    Process_ID = 1
                Else
                    Process_Int = 10000 * Count_Files(root_path + "temp", ".diva") / PV_SUM
                    ProgressBar1.Value = Min(10000, Process_Int)
                End If
            Case 13
                Try
                    If File.Exists(root_path + "temp\ACE.end") Then
                        Main_Timer.Enabled = False
                        read_geiger(root_path + "temp\ace_ER_1.txt", root_path + "temp\v_tree2.tre", "", "ER", False)
                        read_geiger(root_path + "temp\ace_SYM_1.txt", root_path + "temp\v_tree2.tre", "", "SYM", True)
                        read_geiger(root_path + "temp\ace_ARD_1.txt", root_path + "temp\v_tree2.tre", "", "ARD", True)
                        Dim sw As New StreamWriter(root_path + "temp\analysis_result.log")
                        sw.Write(rasp_result)
                        sw.Close()

                        Dim sr As New StreamReader(root_path + "temp\ACE_1.loglik")
                        Dim line As String = sr.ReadToEnd
                        sr.Close()
                        Me.Invoke(RT1, New Object() {line + vbCrLf})

                        Me.Invoke(RT2_S, New Object() {""})
                        ProgressBar1.Value = 0
                        Me.Invoke(RT1, New Object() {"Open [Graphic->Tree View] to see the result" + vbCrLf})
                        Enable_Windows()
                        Process_Text = ""
                        Process_Int = 0
                        Process_ID = -1
                        StartTreeView = True
                        tree_view_title = "APE Analysis Result"
                        Main_Timer.Enabled = True
                        'End If
                    Else
                        ProgressBar1.Value = 6180
                        Me.Invoke(RT2_S, New Object() {"APE package is loading ..."})
                    End If
                Catch ex As Exception
                    Me.Invoke(RT1, New Object() {"Errors!" + vbCrLf})
                    Enable_Windows()
                    Process_Text = ""
                    Process_Int = 0
                    Process_ID = -1
                    Main_Timer.Enabled = True
                End Try
            Case 14
                If File.Exists(root_path + "temp\ACE.state") Then
                    Me.Invoke(RT2_S, New Object() {"APE package is running ..."})
                    Try
                        Dim sr_state As New StreamReader(root_path + "temp\ACE.state")
                        Dim timer_count As Integer = CSng(sr_state.ReadLine())
                        sr_state.Close()
                        ProgressBar1.Value = Min(10000, 10000 * timer_count / SACE_count)
                    Catch ex As Exception

                    End Try
                    If File.Exists(root_path + "temp\ACE.end") Then
                        Dim th_fd_14 As New Thread(AddressOf FD_14_Combine)
                        th_fd_14.Start()
                        Process_Text = ""
                        Process_Int = 0
                        Process_ID = -2
                        ProgressBar1.Value = 0
                        Main_Timer.Enabled = True
                    End If

                Else
                    ProgressBar1.Value = 6180
                    Me.Invoke(RT2_S, New Object() {"APE package is loading ..."})

                End If

            Case -2

            Case -1
                If ProgressBar1.Value <> 0 Then
                    ProgressBar1.Value = 0
                    Process_Int = 0
                End If
                If Process_Text <> "" Then
                    Me.Activate()
                    CmdBox.Focus()
                    CmdBox.AppendText(Process_Text)
                    Process_Text = ""
                End If
                Me.Invoke(RT2_S, New Object() {""})
        End Select
    End Sub
    Public Sub FD_11_Combine()
        CheckForIllegalCrossThreadCalls = False
        Process_Int = 0
        Process_ID = 1
        Me.Invoke(RT1, New Object() {"Process end at " + Date.Now.ToString + vbCrLf})
        Me.Invoke(RT2_S, New Object() {"S-DIVA Analysis ..."})
        do_analysis()
        Me.Invoke(RT2_S, New Object() {""})
        Process_Text += vbCrLf + "Analysis end at " + Date.Now.ToString + vbCrLf
        Process_Int = 0
        Process_ID = -1
        StartTreeView = True
        tree_view_title = "S-DIVA Analysis Result"
        CheckForIllegalCrossThreadCalls = True
    End Sub
    Public Sub FD_14_Combine()

        CheckForIllegalCrossThreadCalls = False
        Try
            Me.Invoke(RT2_S, New Object() {"Converting results ..."})
            Dim sr As StreamReader
            If CheckBox3.Checked Then
                sr = New StreamReader(root_path + "temp\random_trees.tre")
            Else
                sr = New StreamReader(root_path + "temp\clean_num.trees")
            End If
            Dim line As String


            For i As Integer = 1 To SACE_count
                line = sr.ReadLine
                rasp_result = ""
                read_geiger(root_path + "temp\ace_ER_" + i.ToString + ".txt", "", line, "ER", False)
                Dim wr As New StreamWriter(root_path + "temp\rasp_result." + i.ToString + ".ACE_ER.txt")
                wr.Write(rasp_result)
                wr.Close()
                rasp_result = ""
                read_geiger(root_path + "temp\ace_SYM_" + i.ToString + ".txt", "", line, "SYM", False)
                Dim wr1 As New StreamWriter(root_path + "temp\rasp_result." + i.ToString + ".ACE_SYM.txt")
                wr1.Write(rasp_result)
                wr1.Close()
                rasp_result = ""
                read_geiger(root_path + "temp\ace_ARD_" + i.ToString + ".txt", "", line, "ARD", False)
                Dim wr2 As New StreamWriter(root_path + "temp\rasp_result." + i.ToString + ".ACE_ARD.txt")
                wr2.Write(rasp_result)
                wr2.Close()

                Process_Int = CInt(10000 * i / SACE_count)
            Next
            sr.Close()
            Me.Invoke(RT2_S, New Object() {"Combining results ..."})
            comb_result_all("ER", ".ACE_ER.txt", "result_ace_er.txt")
            comb_result_all("SYM", ".ACE_SYM.txt", "result_ace_sym.txt")
            comb_result_all("ARD", ".ACE_ARD.txt", "result_ace_ard.txt")
            connect_result({"result_ace_er.txt", "result_ace_sym.txt", "result_ace_ard.txt"})
            Enable_Windows()
            'CmdBox.AppendText(Process_Text)
            Me.Invoke(RT1, New Object() {"Process end at " + Date.Now.ToString + vbCrLf})
            Me.Invoke(RT1, New Object() {"Open [Graphic->Tree View] to see the result" + vbCrLf})
            Me.Invoke(RT2_S, New Object() {""})
            StartTreeView = True
            tree_view_title = "S-APE Analysis Result"
            Process_Int = 0
            Process_ID = -1
        Catch ex As Exception
            Me.Invoke(RT2_S, New Object() {ex.ToString + vbCrLf})
            Me.Invoke(RT2_S, New Object() {"S-APE analysis failed"})
            Enable_Windows()
            Process_Text = ""
            Process_Int = 0
            Process_ID = -1
            BGB_gen = -1
            Main_Timer.Enabled = True
            Exit Sub
        End Try

        CheckForIllegalCrossThreadCalls = True
    End Sub
    Public Sub FD_10_Combine()

        CheckForIllegalCrossThreadCalls = False
        'Try
        Me.Invoke(RT2_S, New Object() {"Converting results ..."})
            For i As Integer = 1 To SBGB_count
                rasp_result = ""
                read_BGB(root_path + "temp\temp_BGB." + i.ToString + ".tab", root_path + "temp\temp_BGB." + i.ToString + ".tre", "combine", False)
                File.Delete(root_path + "temp\temp_BGB." + i.ToString + ".tab")
                File.Delete(root_path + "temp\temp_BGB." + i.ToString + ".tre")
                File.Delete(root_path + "temp\temp_BGB." + i.ToString + ".data")
                Process_Int = CInt(10000 * i / SBGB_count)
                Dim wr As New StreamWriter(root_path + "temp\rasp_result." + i.ToString + ".BGB.txt")
                wr.Write(rasp_result)
                wr.Close()
            Next
            Me.Invoke(RT2_S, New Object() {"Combining results ..."})
            comb_result_all(BGB_Config.ComboBox1.Text, ".BGB.txt")
            Enable_Windows()
            CmdBox.AppendText(Process_Text)
            Me.Invoke(RT1, New Object() {"Process end at " + Date.Now.ToString + vbCrLf})
            Me.Invoke(RT1, New Object() {"Open [Graphic->Tree View] to see the result" + vbCrLf})
            Me.Invoke(RT2_S, New Object() {""})
            StartTreeView = True
            tree_view_title = "S-BioGenBEARS Analysis Result"
            Process_Int = 0
            Process_ID = -1
            'Catch ex As Exception
            '    Me.Invoke(RT2_S, New Object() {ex.ToString + vbCrLf})
            '    Me.Invoke(RT2_S, New Object() {"S-BioGenBEARS analysis failed"})
            '    Enable_Windows()
            '    Process_Text = ""
            '    Process_Int = 0
            '    Process_ID = -1
            '    BGB_gen = -1
            '    Main_Timer.Enabled = True
            '    Exit Sub
            'End Try

            CheckForIllegalCrossThreadCalls = True
    End Sub
    Public Sub FD_6_Combine()
        Try
            Me.Invoke(RT2_S, New Object() {"Converting results ..."})
            convert_rasp()
            CheckForIllegalCrossThreadCalls = False
            Me.Invoke(RT2_S, New Object() {"Combining results ..."})
            comb_result_all("SDEC", ".DEC.txt")
            Enable_Windows()
            CmdBox.AppendText(Process_Text)
            Me.Invoke(RT1, New Object() {"Process end at " + Date.Now.ToString + vbCrLf})
            Me.Invoke(RT1, New Object() {"Open [Graphic->Tree View] to see the result" + vbCrLf})
            Me.Invoke(RT2_S, New Object() {""})
            StartTreeView = True
            tree_view_title = "S-DEC Analysis Result"
            Process_Int = 0
            Process_ID = -1
            CheckForIllegalCrossThreadCalls = True
        Catch ex As Exception
            Me.Invoke(RT2_S, New Object() {ex.ToString + vbCrLf})
        Me.Invoke(RT2_S, New Object() {"S-DEC analysis failed"})
        Enable_Windows()
        Process_Text = ""
        Process_Int = 0
        Process_ID = -1
        BGB_gen = -1
        Main_Timer.Enabled = True
        Exit Sub
        End Try
    End Sub
    Public Sub convert_rasp()
        For i As Integer = 1 To sdec_count
            If File.Exists(root_path + "temp" + path_char + "result." + i.ToString + ".dec") Then
                Dim sr As New StreamReader(root_path + "temp" + path_char + "temp_lg." + i.ToString + ".tree")
                read_DEC(root_path + "temp" + path_char + "result." + i.ToString + ".dec", sr.ReadLine)
                sr.Close()
                For j As Integer = 1 To taxon_num - 1
                    rasp_result += "node " + (j + taxon_num).ToString + " (LR):" + node_value(j) + Chr(13) + vbCrLf
                Next
                Dim wr As New StreamWriter(root_path + "temp" + path_char + "rasp_result." + i.ToString + ".DEC.txt")
                wr.Write(rasp_result)
                wr.Close()
                File.Delete(root_path + "temp" + path_char + "temp_lg." + i.ToString + ".tre.bgkey.tre")
                File.Delete(root_path + "temp" + path_char + "temp_lg." + i.ToString + ".tre.bgsplits.tre")
                File.Delete(root_path + "temp" + path_char + "temp_lg." + i.ToString + ".tre.bgstates.tre")
                File.Delete(root_path + "temp" + path_char + "result." + i.ToString + ".dec")
                File.Delete(root_path + "temp" + path_char + "temp_lg." + i.ToString + ".lg")
                File.Delete(root_path + "temp" + path_char + "temp_lg." + i.ToString + ".data")
                File.Delete(root_path + "temp" + path_char + "temp_lg." + i.ToString + ".rm")
                File.Delete(root_path + "temp" + path_char + "temp_lg." + i.ToString + ".tre")
                File.Delete(root_path + "temp" + path_char + "temp_lg." + i.ToString + ".tree")
                Process_Int = CInt(10000 * i / sdec_count)
            End If
        Next
    End Sub

    Dim taxon_list() As String
    Dim taxon_dis() As String
    Dim tree_lg As String
    Public Sub read_tree(ByVal treeline As String)
        Dim tree_Temp As String = treeline.Substring(treeline.IndexOf("("), treeline.Length - treeline.IndexOf("(")).Replace(";", "")
        Dim tree_complete As String = ""
        Dim is_sym As Boolean = False
        Dim is_sym1 As Boolean = False
        Dim temp_node_line As String = ""
        For Each tree_chr As Char In tree_Temp
            If tree_chr = ")" Then
                is_sym = True
            End If
            If tree_chr = ":" Then
                is_sym = False
            End If
            If is_sym Then
                temp_node_line += tree_chr
            End If
        Next
        node_list = temp_node_line.Split(New Char() {")"c})
        is_sym = False
        For Each tree_chr As Char In tree_Temp
            If tree_chr = ":" Then
                is_sym = True
            End If
            If tree_chr = "," Or tree_chr = "(" Or tree_chr = ")" Then
                is_sym = False
            End If
            If tree_chr = ")" Then
                tree_complete = tree_complete + tree_chr.ToString
                is_sym = True
            End If
            If is_sym = False Then
                tree_complete = tree_complete + tree_chr.ToString
            End If
        Next
        taxon_num = tree_complete.Length - tree_complete.Replace(",", "").Length + 1
        ReDim taxon_list(taxon_num)
        Dim t_id As Integer = 1
        is_sym = True
        tree_Temp = tree_complete
        tree_complete = ""
        For Each tree_chr As Char In tree_Temp
            If tree_chr = "," Or tree_chr = "(" Or tree_chr = ")" Or tree_chr = ")" Then
                If is_sym = False Then
                    tree_complete = tree_complete + t_id.ToString + tree_chr.ToString
                    t_id += 1

                Else
                    tree_complete = tree_complete + tree_chr.ToString
                End If
                is_sym = True
            Else
                taxon_list(t_id) += tree_chr
                is_sym = False
            End If
        Next
        tree_lg = tree_complete

        read_node(tree_lg, taxon_num)

    End Sub
    Public Sub read_DEC(ByVal path As String, ByVal treeline As String)
        Dim sr As New StreamReader(path)
        Dim line As String = treeline
        Dim taxon_line As String = ""
        rasp_result = "DEC result file of " + state_header + vbCrLf

        rasp_result += "[TAXON]" + vbCrLf

        'read_tree(treeline)
        Do
            taxon_line = sr.ReadLine
        Loop Until taxon_line.StartsWith("Reading species")
        For i As Integer = 1 To taxon_num
            rasp_result += i.ToString + "	" + dtView.Item(i - 1).Item(1).ToString + "	" + DataGridView1.Rows(i - 1).Cells(state_index + 1).Value + vbCrLf
        Next
        For i As Integer = 1 To taxon_num
            line = line.Replace("(" + dtView.Item(i - 1).Item(0).ToString + ",", "($%*" + i.ToString + "$%*,")
            line = line.Replace("," + dtView.Item(i - 1).Item(0).ToString + ")", ",$%*" + i.ToString + "$%*)")
            line = line.Replace("," + dtView.Item(i - 1).Item(0).ToString + ",", ",$%*" + i.ToString + "$%*,")
            line = line.Replace("(" + dtView.Item(i - 1).Item(0).ToString + ":", "($%*" + i.ToString + "$%*:")
            line = line.Replace("," + dtView.Item(i - 1).Item(0).ToString + ":", ",$%*" + i.ToString + "$%*:")
            line = line.Replace("," + dtView.Item(i - 1).Item(0).ToString + ":", ",$%*" + i.ToString + "$%*:")
        Next
        line = line.Replace("$%*", "")
        rasp_result += "[TREE]" + vbCrLf + "Tree=" + line + vbCrLf

        'ReDim node_id(taxon_num - 1)
        ReDim node_value(taxon_num - 1)
        'For i As Integer = 1 To taxon_num - 1
        '    node_id(i) = CInt(node(i, 3))
        'Next
        rasp_result += "[RESULT]" + vbCrLf + "DEC results:" + vbCrLf
        Do
            If line.ToUpper.StartsWith("Ancestral states".ToUpper) Then
                Dim L_id As Integer = CInt(line.Split(New Char() {"	"c})(1))
                'Dim id As Integer = Array.IndexOf(node_id, L_id)
                line = sr.ReadLine
                Dim temp_range(0) As String
                Dim temp_value(0) As Single
                Dim temp_sum As Single = 0
                Do
                    If line <> "" Then
                        If line.Split(New Char() {"	"c}).Length > 2 Then
                            If IsNumeric(line.Split(New Char() {"	"c})(2)) Then
                                ReDim Preserve temp_range(temp_range.Length)
                                temp_range(UBound(temp_range)) = line.Split(New Char() {"	"c})(1).Replace("_", "")
                                ReDim Preserve temp_value(temp_value.Length)
                                temp_value(UBound(temp_value)) = Val(line.Split(New Char() {"	"c})(3).Replace("(", "").Replace(")", ""))
                                temp_sum += temp_value(UBound(temp_value))
                            End If
                        End If
                        line = sr.ReadLine
                    End If
                Loop Until line = ""
                If temp_sum > 0 Then
                    Dim temp_min As Single = temp_value(1)
                    For i As Integer = 1 To UBound(temp_value)
                        If temp_value(i) < temp_min Then
                            temp_min = temp_value(i)
                        End If
                    Next
                    For i As Integer = 1 To UBound(temp_value)
                        temp_value(i) = temp_value(i) - temp_min
                    Next
                    For i As Integer = 1 To UBound(temp_value)
                        temp_value(i) = Exp(-temp_value(i))
                    Next
                    temp_sum = 0
                    For i As Integer = 1 To UBound(temp_value)
                        temp_sum += temp_value(i)
                    Next
                    For i As Integer = 1 To UBound(temp_value)
                        node_value(L_id) += " " + temp_range(i) + " " + (temp_value(i) / temp_sum * 100).ToString("F2")
                    Next
                End If
            End If
            line = sr.ReadLine
        Loop Until line Is Nothing
        sr.Close()
        'Array.Sort(node_id, node_value)
    End Sub
    Dim final_result_list() As String
    Dim final_p_list() As Integer
    Dim final_value_list() As Single
    Dim node_line() As String
    Dim D_id() As Integer
    Dim TaxonName() As String
    Public Sub comb_result_all(ByVal title As String, ByVal file_name As String, Optional ByVal save_name As String = "analysis_result.log")
        Dim Treeline As String
        Treeline = tree_show_with_value.Replace(";", "")
        Dim NumofTaxon As Integer = Treeline.Length - Treeline.Replace(",", "").Length + 1
        Dim NumofNode As Integer = Treeline.Length - Treeline.Replace("(", "").Length
        ReDim final_result_list(NumofNode - 1)
        ReDim final_p_list(NumofNode - 1)
        ReDim final_value_list(NumofNode - 1)
        get_tree_length(Treeline)
        ReDim node_line(NumofNode - 1)
        For i As Integer = 0 To NumofNode - 1
            node_line(i) = ""
            final_result_list(i) = " "
            final_p_list(i) = 0
            final_value_list(i) = 0
            node_line(i) = "#"
            Dim temp_array() As String = Poly_Node(i, 3).Split(New Char() {","c})
            Array.Sort(temp_array)
            For Each j As String In temp_array
                If j <> "" Then
                    node_line(i) += j + "#"
                End If
            Next
        Next
        Dim file_id As Integer = 1
        Dim temp_id As Integer = 0

        Dim fileList() As String = Directory.GetFiles(root_path + "temp")  ' 遍历temp所有的文件 
        For Each FileName As String In fileList
            Try
                If FileName.ToUpper.EndsWith(file_name.ToUpper) Then
                    Dim temp_path As String = FileName

                    Dim sr As StreamReader
                    sr = New StreamReader(temp_path)
                    Dim line As String = ""
                    Do
                        line = sr.ReadLine

                    Loop Until line.StartsWith("[TAXON]")
                    line = sr.ReadLine
                    Dim Temparray As Integer = 1
                    ReDim D_id(NumofTaxon - 1)
                    ReDim TaxonName(NumofTaxon - 1)
                    TaxonName(0) = line.Split(New Char() {"	"c})(1)
                    Dim temp_range As String = ""
                    line = sr.ReadLine
                    Do
                        TaxonName(Temparray) = line.Split(New Char() {"	"c})(1)
                        Temparray += 1
                        line = sr.ReadLine
                    Loop Until line.StartsWith("[TREE]")

                    For i As Integer = 0 To NumofTaxon - 1
                        D_id(i) = Array.IndexOf(TaxonName, dtView.Item(i).Item(1).ToString) + 1
                        If D_id(i) <= 0 Then
                            MsgBox("Can not find " + dtView.Item(i).Item(1).ToString)
                            Exit For
                        End If
                    Next

                    Treeline = sr.ReadLine.Split(New Char() {"="c})(1).Replace(";", "")
                    NumofTaxon = Treeline.Length - Treeline.Replace(",", "").Length + 1
                    NumofNode = Treeline.Length - Treeline.Replace("(", "").Length
                    get_tree_length(Treeline)
                    Dim temp_node_line() As String
                    ReDim temp_node_line(NumofNode - 1)
                    For i As Integer = 0 To NumofNode - 1
                        temp_node_line(i) = "#"
                        Dim temp_array() As String = Poly_Node(i, 3).Split(New Char() {","c})
                        For k As Integer = 0 To UBound(temp_array)
                            If temp_array(k) <> "" Then
                                temp_array(k) = (Array.IndexOf(D_id, CInt(temp_array(k))) + 1).ToString
                                If temp_array(k) = 0 Then
                                    MsgBox("error!!")
                                End If
                            End If
                        Next
                        Array.Sort(temp_array)

                        For Each j As String In temp_array
                            If j <> "" Then
                                temp_node_line(i) += j + "#"
                            End If
                        Next
                    Next

                    Dim temp_result_list() As String
                    ReDim temp_result_list(NumofNode - 1)
                    temp_id = 0
                    Do
                        If line.StartsWith("node") Then
                            temp_result_list(temp_id) = line.Split(New Char() {":"c})(1)
                            temp_id += 1
                        End If
                        line = sr.ReadLine
                    Loop Until line Is Nothing
                    sr.Close()
                    For temp_id = 0 To NumofNode - 1
                        Dim temp_node_id As Integer = Array.IndexOf(node_line, temp_node_line(temp_id))
                        If temp_node_id >= 0 Then
                            Dim temp_p1() As String = temp_result_list(temp_id).Split(New Char() {" "c})
                            Dim temp_p2() As String = final_result_list(temp_node_id).Split(New Char() {" "c})
                            For k As Integer = 1 To UBound(temp_p1) Step 2
                                Dim temp_p_id As Integer = Array.IndexOf(temp_p2, temp_p1(k))
                                If temp_p_id < 0 Then
                                    ReDim Preserve temp_p2(UBound(temp_p2) + 2)
                                    temp_p2(UBound(temp_p2) - 1) = temp_p1(k)
                                    temp_p2(UBound(temp_p2)) = temp_p1(k + 1)
                                Else
                                    temp_p2(temp_p_id + 1) = (Val(temp_p2(temp_p_id + 1)) + Val(temp_p1(k + 1))).ToString("F2")
                                End If
                            Next
                            final_result_list(temp_node_id) = ""
                            For Each l As String In temp_p2
                                If l <> "" Then
                                    final_result_list(temp_node_id) += " " + l
                                End If
                            Next
                            final_p_list(temp_node_id) += 1
                        End If
                    Next
                    file_id += 1

                End If
            Catch ex As Exception
                Me.Invoke(RT2_S, New Object() {ex.ToString + vbCrLf})
            End Try
        Next

        Treeline = tree_show_with_value.Replace(";", "")
        NumofTaxon = Treeline.Length - Treeline.Replace(",", "").Length + 1
        NumofNode = Treeline.Length - Treeline.Replace("(", "").Length
        For i As Integer = 0 To NumofNode - 1
            Dim temp_p1() As String = final_result_list(i).Split(New Char() {" "c})
            For k As Integer = 2 To UBound(temp_p1) Step 2
                final_value_list(i) += Val(temp_p1(k))
            Next
        Next
        For i As Integer = 0 To NumofNode - 1
            Dim temp_p1() As String = final_result_list(i).Split(New Char() {" "c})
            Dim temp_a() As String
            Dim temp_p() As Single
            ReDim temp_a(UBound(temp_p1) / 2)
            ReDim temp_p(UBound(temp_p1) / 2)
            For k As Integer = 2 To UBound(temp_p1) Step 2
                temp_p(k / 2) = Val(temp_p1(k)) / final_p_list(i)
                temp_a(k / 2) = temp_p1(k - 1)
            Next
            Array.Sort(temp_p, temp_a)
            final_result_list(i) = ""
            For l As Integer = 0 To UBound(temp_a)
                If temp_a(l) <> "" Then
                    final_result_list(i) = " " + temp_a(l) + " " + temp_p(l).ToString("F2") + final_result_list(i)
                End If
            Next
        Next

        Dim swf As New StreamWriter(root_path + "temp\" + save_name, False)
        swf.WriteLine("Combined result file")
        swf.WriteLine("[TAXON]")
        For i As Integer = 1 To dtView.Count
            swf.WriteLine(dtView.Item(i - 1).Item(0).ToString + "	" + dtView.Item(i - 1).Item(1).ToString + "	" + dtView.Item(i - 1).Item(state_index).ToString.ToUpper)
        Next
        swf.WriteLine("[TREE]")
        swf.WriteLine("Tree=" + tree_show_with_value)
        swf.WriteLine("[RESULT]")
        swf.WriteLine(title + " results:")
        For i As Integer = 0 To NumofNode - 1
            Dim t_list() As String = Poly_Node(i, 3).Split(New Char() {","c})
            swf.WriteLine("node " + (NumofTaxon + i + 1).ToString + " (anc. of terminals " + t_list(0) + "-" + t_list(UBound(t_list)) + "):" + final_result_list(i))
            Me.Invoke(RT1, New Object() {"node " + (NumofTaxon + i + 1).ToString + " (anc. of terminals " + t_list(0) + "-" + t_list(UBound(t_list)) + "):" + final_result_list(i) + vbCrLf})
        Next
        swf.Close()
    End Sub
    Public Sub connect_result(ByVal files() As String, Optional ByVal save_name As String = "analysis_result.log")
        Dim sw As New StreamWriter(root_path + "temp\" + save_name)
        Dim sr1 As New StreamReader(root_path + "temp\" + files(0))
        Dim line As String = sr1.ReadLine
        Do
            sw.WriteLine(line)
            line = sr1.ReadLine
        Loop Until line Is Nothing
        sr1.Close()
        For i As Integer = 1 To UBound(files)
            Dim sr As New StreamReader(root_path + "temp\" + files(i))
            Do While line <> "[RESULT]"
                line = sr.ReadLine
            Loop
            line = sr.ReadLine
            Do
                sw.WriteLine(line)
                line = sr.ReadLine
            Loop Until line Is Nothing
            sr.Close()
        Next
        sw.Close()
    End Sub

    Private Sub QuickLoadToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QuickLoadToolStripMenuItem.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Beast Trees (*.trees)|*.trees;*.TREES|Mrbayes Tree Data (*.t)|*.t;*.T|ALL Files(*.*)|*.*"
        opendialog.FileName = ""
        opendialog.Multiselect = False
        opendialog.DefaultExt = ".trees"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            Me.Activate()
            Disable_Windows()
            CloseDataToolStripMenuItem.Enabled = True
            tree_path = opendialog.FileName
            error_no = load_names(opendialog.FileName.Substring(opendialog.FileName.LastIndexOf(".") + 1))
            Process_Text += "Loading Trees Dataset ... " + vbCrLf
            If error_no = 0 Then
                If opendialog.FileName.ToLower.EndsWith(".trees") Then
                    Dim l_Tree As New Thread(AddressOf load_beast_trees)
                    l_Tree.CurrentCulture = ci
                    l_Tree.Start()
                Else
                    Dim l_Tree As New Thread(AddressOf load_bayes_trees)
                    l_Tree.CurrentCulture = ci
                    l_Tree.Start()
                End If
            End If
            If File.Exists(root_path + "temp" + path_char + "clean_num.trees") Then
                File.Delete(root_path + "temp" + path_char + "clean_num.trees")
            End If
            set_style("PLEASE CHECK THE STATUS BEFORE ANALYSIS", 4)
        End If
    End Sub

    Public Sub load_beast_trees()
        Try
            Dim line As String = ""
            Dim rt As StreamReader
            Dim B_WT As StreamWriter
            Dim P_WT As StreamWriter
            rt = New StreamReader(tree_path)
            If B_Tree_File = "" Then
                B_Tree_File = "temp" + path_char + format_time(Date.Now.ToString) + "_B.tre"
                P_Tree_File = "temp" + path_char + format_time(Date.Now.ToString) + "_P.tre"
                omittedtree = B_Tree_File
                B_WT = New StreamWriter(root_path + B_Tree_File, False)
                P_WT = New StreamWriter(root_path + P_Tree_File, False)
            Else
                B_WT = New StreamWriter(root_path + B_Tree_File, True)
                P_WT = New StreamWriter(root_path + P_Tree_File, True)
            End If
            Dim wt_clean_num As New StreamWriter(root_path + "temp" + path_char + "clean_num.trees", True)
            Dim wt_clean_num_p As New StreamWriter(root_path + "temp" + path_char + "clean_num_p.trees", True)
            line = rt.ReadLine
            Process_ID = 0
            Do While line Is Nothing = False
                If line.ToUpper.Replace("	", "").StartsWith("TREE") Then
                    Do While line.Contains(";") = False
                        Dim next_tree_line As String = rt.ReadLine
                        If next_tree_line <> "" Then
                            line = line + next_tree_line
                        End If
                    Loop
                    Dim tree_Temp As String = line.Substring(line.IndexOf("("), line.Length - line.IndexOf("("))
                    Dim tree_complete As String = ""
                    Dim tree_sdec As String = ""
                    Dim is_sym As Boolean = False
                    Dim is_sym1 As Boolean = False
                    For Each tree_chr As Char In tree_Temp
                        If tree_chr = "[" Then
                            is_sym1 = True
                            is_sym = True
                        End If
                        If tree_chr = "]" Then
                            is_sym1 = False
                        End If
                        If tree_chr = ":" Then
                            is_sym = True
                        End If
                        If (tree_chr = "," Or tree_chr = "(" Or tree_chr = ")") And is_sym1 = False Then
                            is_sym = False
                        End If
                        If tree_chr = ")" Then
                            tree_complete = tree_complete + tree_chr.ToString
                            is_sym = True
                        End If
                        If is_sym = False Then
                            tree_complete &= tree_chr.ToString
                        End If
                        If is_sym1 = False And tree_chr <> "]" Then
                            tree_sdec &= tree_chr.ToString
                        End If
                    Next
                    Dim outgroup_str As String = ""
                    Dim isbase_three As Boolean = True
                    If tree_complete.EndsWith(";") = False Then
                        tree_complete = tree_complete.Replace(" ", "") + ";"
                    End If
                    If tree_sdec.EndsWith(";") = False Then
                        tree_sdec = tree_sdec.Replace(" ", "") + ";"
                    End If
                    If tree_complete.Replace("(", "").Length = tree_complete.Replace(",", "").Length Then
                        B_WT.WriteLine(tree_complete)
                        P_WT.WriteLine(tree_complete)
                        Tree_Num_B += 1
                        wt_clean_num.WriteLine(tree_sdec)
                        wt_clean_num_p.WriteLine(tree_sdec)
                        Process_Gen1 = Tree_Num_B
                    Else
                        wt_clean_num_p.WriteLine(tree_sdec)
                        P_WT.WriteLine(tree_complete)
                    End If
                    Tree_Num_P += 1
                    Process_Gen = Tree_Num_P
                End If
                line = rt.ReadLine()
            Loop
            Me.Invoke(TB1, New Object() {Tree_Num_B.ToString})
            Me.Invoke(TB3, New Object() {Tree_Num_P.ToString})
            Process_Text += "Load Trees Dataset Successfully!" + vbCrLf
            Process_ID = -1
            rt.Close()
            B_WT.Close()
            P_WT.Close()
            wt_clean_num.Close()
            wt_clean_num_p.Close()
            Enable_Windows()
            Enable_Buttun()
        Catch ex As Exception
            MsgBox(ex.ToString)
            MsgBox("Cannot format the trees 3!")
            Enable_Windows()
            Exit Sub
        End Try
    End Sub
    Public Sub load_bayes_trees()
        Try
            Dim line As String = ""
            Dim rt As StreamReader
            Dim B_WT As StreamWriter
            Dim P_WT As StreamWriter
            Dim have_translate As Boolean = False
            rt = New StreamReader(tree_path)
            If B_Tree_File = "" Then
                B_Tree_File = "temp" + path_char + format_time(Date.Now.ToString) + "_B.tre"
                P_Tree_File = "temp" + path_char + format_time(Date.Now.ToString) + "_P.tre"
                omittedtree = B_Tree_File
                B_WT = New StreamWriter(root_path + B_Tree_File, False)
                P_WT = New StreamWriter(root_path + P_Tree_File, False)
            Else
                B_WT = New StreamWriter(root_path + B_Tree_File, True)
                P_WT = New StreamWriter(root_path + P_Tree_File, True)
            End If
            Dim wt_clean_num As New StreamWriter(root_path + "temp" + path_char + "clean_num.trees", True)
            Dim wt_clean_num_p As New StreamWriter(root_path + "temp" + path_char + "clean_num_p.trees", True)
            line = rt.ReadLine

            Dim f_t_name(,) As String
            ReDim f_t_name(dtView.Count - 1, 1)
            Process_ID = 0
            Do While line Is Nothing = False

                Do
                    If line.StartsWith("	") Or line.StartsWith(" ") Then
                        line = line.Remove(0, 1)
                    Else
                        Exit Do
                    End If
                Loop


                If line.Replace("	", "").ToUpper.StartsWith("TRANSLATE") Then
                    line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                    Dim name_num As Integer = 0
                    Do

                        If line.Length > 0 Then
                            Do
                                If line.StartsWith("	") Or line.StartsWith(" ") Then
                                    line = line.Remove(0, 1)
                                Else
                                    Exit Do
                                End If
                            Loop
                            Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                            f_t_name(name_num, 0) = TRANSLATE(0)
                            f_t_name(name_num, 1) = TRANSLATE(1).Replace("'", "")
                            name_num = name_num + 1
                        End If
                        line = rt.ReadLine.Replace("	", " ").Replace(",", "")
                    Loop Until line.Contains(";")
                    If line.Replace("	", "").Replace(" ", "").Length > 1 Then
                        Do
                            If line.StartsWith("	") Or line.StartsWith(" ") Then
                                line = line.Remove(0, 1)
                            Else
                                Exit Do
                            End If
                        Loop
                        Dim TRANSLATE() As String = line.Replace(";", "").Split(New Char() {" "c})
                        f_t_name(name_num, 0) = TRANSLATE(0)
                        f_t_name(name_num, 1) = TRANSLATE(1).Replace("'", "")
                        name_num = name_num + 1
                    End If
                End If

                If line.Replace("	", "").ToUpper.StartsWith("TREE") Or line.Replace("	", "").ToUpper.StartsWith("(") Then
                    Do While line.Contains(";") = False
                        Dim next_tree_line As String = rt.ReadLine
                        If next_tree_line <> "" Then
                            line = line + next_tree_line
                        End If
                    Loop
                    Dim tree_Temp As String = line.Substring(line.IndexOf("("), line.Length - line.IndexOf("("))
                    Dim tree_complete As String = ""
                    Dim tree_sdec As String = ""
                    Dim is_sym As Boolean = False
                    Dim is_sym1 As Boolean = False
                    For Each tree_chr As Char In tree_Temp
                        If tree_chr = "[" Then
                            is_sym1 = True
                            is_sym = True
                        End If
                        If tree_chr = "]" Then
                            is_sym1 = False
                        End If
                        If tree_chr = ":" Then
                            is_sym = True
                        End If
                        If (tree_chr = "," Or tree_chr = "(" Or tree_chr = ")") And is_sym1 = False Then
                            is_sym = False
                        End If
                        If tree_chr = ")" Then
                            tree_complete = tree_complete + tree_chr.ToString
                            is_sym = True
                        End If
                        If is_sym = False Then
                            tree_complete &= tree_chr.ToString
                        End If
                        If is_sym1 = False And tree_chr <> "]" Then
                            tree_sdec &= tree_chr.ToString
                        End If
                    Next
                    If dtView.Count <= 1 Then
                        taxon_line = tree_complete
                        Make_Taxon_List()
                    End If
                    Dim outgroup_str As String = ""
                    Dim isbase_three As Boolean = True
                    If tree_complete.Replace("(", "").Length - tree_complete.Replace(",", "").Length = 1 Then
                        Dim tree_poly() As Char = tree_complete
                        ReDim tree_char(taxon_num * 4)
                        tree_complete = ""
                        Dim char_id As Integer = 0
                        Dim l_c As Integer = 0
                        Dim r_c As Integer = 0
                        Dim dh As Integer = 0
                        Dim last_symb As Boolean = True
                        For i As Integer = 0 To tree_poly.Length - 1
                            Select Case tree_poly(i)
                                Case "("
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True

                                Case ")"
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True
                                Case ","
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True
                                Case Else
                                    If last_symb Then
                                        char_id += 1
                                        tree_char(char_id) = tree_poly(i)
                                        last_symb = False
                                    Else
                                        tree_char(char_id) += tree_poly(i)
                                    End If
                            End Select
                        Next
                        Dim three_clade_id(2) As Integer
                        three_clade_id(0) = 0
                        three_clade_id(1) = 0
                        three_clade_id(2) = 0
                        For i As Integer = 1 To tree_char.Length - 1
                            If tree_char(i) = "(" Then
                                l_c = l_c + 1
                            End If
                            If tree_char(i) = ")" Then
                                three_clade_id(2) = i
                            End If
                            If tree_char(i) = "," Then
                                dh = dh + 1
                            End If
                            If dh = l_c + 1 Then
                                If three_clade_id(1) = 0 Then
                                    dh = 0
                                    l_c = 0
                                End If
                                three_clade_id(1) = i
                            End If
                        Next
                        If dh <> l_c Then
                            isbase_three = False
                        End If
                        dh = 0
                        l_c = 0
                        For i As Integer = 0 To three_clade_id(2) - 1
                            If tree_char(three_clade_id(2) - i) = ")" Then
                                r_c = r_c + 1
                            End If
                            If tree_char(three_clade_id(2) - i) = "," Then
                                dh = dh + 1
                            End If
                            If dh = r_c + 1 Then
                                If three_clade_id(0) = 0 Then
                                    dh = 0
                                    r_c = 0
                                End If
                                three_clade_id(0) = three_clade_id(2) - i
                            End If
                        Next
                        If dh <> r_c Then
                            isbase_three = False
                        End If
                        dh = 0
                        r_c = 0
                        For i As Integer = 0 To three_clade_id(2) - 1
                            Select Case tree_char(i)
                                Case "("
                                    l_c += 1
                                Case ","
                                    dh = dh + 1
                                Case ")"
                                    r_c += 1
                                Case Else

                            End Select
                            If i = three_clade_id(0) Then
                                If l_c <> dh Or dh <> r_c + 1 Then
                                    isbase_three = False
                                End If
                            End If
                            If i = three_clade_id(1) Then
                                If l_c <> dh - 1 Or dh - 1 <> r_c + 1 Then
                                    isbase_three = False
                                End If
                            End If
                        Next
                        If isbase_three Then
                            If three_clade_id(2) - three_clade_id(1) <= three_clade_id(0) - 1 Then
                                If three_clade_id(2) - three_clade_id(1) <= three_clade_id(1) - three_clade_id(0) Then
                                    tree_complete = "("
                                    For i As Integer = 1 To three_clade_id(1) - 1
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += "),"
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ";"
                                Else
                                    tree_complete = "("
                                    For i As Integer = three_clade_id(0) To three_clade_id(1) - 1
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ","
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ");"
                                End If
                            Else
                                If three_clade_id(0) - 1 <= three_clade_id(1) - three_clade_id(0) Then
                                    tree_complete = ""
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += "("
                                    For i As Integer = three_clade_id(0) + 1 To three_clade_id(2)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ");"
                                Else
                                    tree_complete = "("
                                    For i As Integer = three_clade_id(0) + 1 To three_clade_id(1) - 1
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ","
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_complete = tree_complete + tree_char(i)
                                    Next
                                    tree_complete += ");"
                                End If
                            End If
                        Else
                            For i As Integer = 1 To three_clade_id(2)
                                tree_complete = tree_complete + tree_char(i)
                            Next
                        End If
                    End If
                    If tree_complete.EndsWith(";") = False Then
                        tree_complete = tree_complete + ";"
                    End If
                    tree_complete = tree_complete.Replace(" ", "")
                    If tree_sdec.Replace("(", "").Length - tree_sdec.Replace(",", "").Length = 1 Then
                        Dim tree_poly() As Char = tree_sdec
                        ReDim tree_char(taxon_num * 5)
                        tree_sdec = ""
                        Dim char_id As Integer = 0
                        Dim l_c As Integer = 0
                        Dim r_c As Integer = 0
                        Dim dh As Integer = 0
                        Dim last_symb As Boolean = True
                        For i As Integer = 0 To tree_poly.Length - 1
                            Select Case tree_poly(i)
                                Case "("
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True

                                Case ")"
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True
                                Case ","
                                    char_id += 1
                                    tree_char(char_id) = tree_poly(i)
                                    last_symb = True
                                Case Else
                                    If last_symb Then
                                        char_id += 1
                                        tree_char(char_id) = tree_poly(i)
                                        last_symb = False
                                    Else
                                        tree_char(char_id) += tree_poly(i)
                                    End If
                            End Select
                        Next
                        Dim three_clade_id(2) As Integer
                        three_clade_id(0) = 0
                        three_clade_id(1) = 0
                        three_clade_id(2) = 0
                        For i As Integer = 1 To tree_char.Length - 1
                            If tree_char(i) = "(" Then
                                l_c = l_c + 1
                            End If
                            If tree_char(i) = ")" Then
                                three_clade_id(2) = i
                            End If
                            If tree_char(i) = "," Then
                                dh = dh + 1
                            End If
                            If dh = l_c + 1 Then
                                If three_clade_id(1) = 0 Then
                                    dh = 0
                                    l_c = 0
                                End If
                                three_clade_id(1) = i
                            End If
                        Next
                        If dh <> l_c Then
                            isbase_three = False
                        End If
                        dh = 0
                        l_c = 0
                        For i As Integer = 0 To three_clade_id(2) - 1
                            If tree_char(three_clade_id(2) - i) = ")" Then
                                r_c = r_c + 1
                            End If
                            If tree_char(three_clade_id(2) - i) = "," Then
                                dh = dh + 1
                            End If
                            If dh = r_c + 1 Then
                                If three_clade_id(0) = 0 Then
                                    dh = 0
                                    r_c = 0
                                End If
                                three_clade_id(0) = three_clade_id(2) - i
                            End If
                        Next
                        If dh <> r_c Then
                            isbase_three = False
                        End If
                        dh = 0
                        r_c = 0
                        For i As Integer = 0 To three_clade_id(2) - 1
                            Select Case tree_char(i)
                                Case "("
                                    l_c += 1
                                Case ","
                                    dh = dh + 1
                                Case ")"
                                    r_c += 1
                                Case Else

                            End Select
                            If i = three_clade_id(0) Then
                                If l_c <> dh Or dh <> r_c + 1 Then
                                    isbase_three = False
                                End If
                            End If
                            If i = three_clade_id(1) Then
                                If l_c <> dh - 1 Or dh - 1 <> r_c + 1 Then
                                    isbase_three = False
                                End If
                            End If
                        Next
                        If isbase_three Then
                            If three_clade_id(2) - three_clade_id(1) <= three_clade_id(0) - 1 Then
                                If three_clade_id(2) - three_clade_id(1) <= three_clade_id(1) - three_clade_id(0) Then
                                    tree_sdec = "("
                                    For i As Integer = 1 To three_clade_id(1) - 1
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += "):0,"
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ";"
                                Else
                                    tree_sdec = "("
                                    For i As Integer = three_clade_id(0) To three_clade_id(1) - 1
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ","
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ");"
                                End If
                            Else
                                If three_clade_id(0) - 1 <= three_clade_id(1) - three_clade_id(0) Then
                                    tree_sdec = ""
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += "("
                                    For i As Integer = three_clade_id(0) + 1 To three_clade_id(2)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ");"
                                Else
                                    tree_sdec = "("
                                    For i As Integer = three_clade_id(0) + 1 To three_clade_id(1) - 1
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ","
                                    For i As Integer = 1 To three_clade_id(0)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    For i As Integer = three_clade_id(1) + 1 To three_clade_id(2)
                                        tree_sdec = tree_sdec + tree_char(i)
                                    Next
                                    tree_sdec += ");"
                                End If
                            End If
                        Else
                            For i As Integer = 1 To three_clade_id(2)
                                tree_sdec = tree_sdec + tree_char(i)
                            Next
                        End If
                    End If
                    If tree_sdec.EndsWith(";") = False Then
                        tree_sdec = tree_sdec + ";"
                    End If
                    tree_sdec = tree_sdec.Replace(" ", "")

                    If tree_complete.Replace("(", "").Length <> tree_complete.Replace(")", "").Length Then
                        MsgBox("Error 10. missing parentheses in tree! Please check you tree file!")
                        Try
                            rt.Close()
                            B_WT.Close()
                            P_WT.Close()
                            Enable_Windows()
                            Enable_Buttun()
                        Catch ex As Exception

                        End Try
                    End If
                    If tree_complete.Replace("(", "").Length = tree_complete.Replace(",", "").Length Then
                        wt_clean_num.WriteLine(tree_sdec)
                        wt_clean_num_p.WriteLine(tree_sdec)
                        B_WT.WriteLine(tree_complete)
                        P_WT.WriteLine(tree_complete)
                        Tree_Num_B += 1
                        Process_Gen1 = Tree_Num_B
                    Else
                        wt_clean_num_p.WriteLine(tree_sdec)
                        P_WT.WriteLine(tree_complete)
                    End If
                    Tree_Num_P += 1
                    Process_Gen = Tree_Num_P
                End If
                line = rt.ReadLine()
            Loop

            Me.Invoke(TB1, New Object() {Tree_Num_B.ToString})
            Me.Invoke(TB3, New Object() {Tree_Num_P.ToString})
            Process_Text += "Load Trees Dataset Successfully!" + vbCrLf
            Process_ID = -1
            rt.Close()
            B_WT.Close()
            P_WT.Close()
            wt_clean_num.Close()
            wt_clean_num_p.Close()
            If Tree_Num_P = 1 Then
                load_final_trees()
                Enable_BBM = True
            End If
            Enable_Windows()
            Enable_Buttun()
        Catch ex As Exception
            MsgBox(ex.ToString)
            MsgBox("Error 6. Cannot format the trees!")
            Enable_Windows()
        End Try
    End Sub

    Private Sub LoadOneTreeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadOneTreeToolStripMenuItem.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Trees (*.trees;*.t;*.tre;*.con)|*.trees;*.TREES;*.t;*.T;*.tre;*.TRE;*.con;*.CON|ALL Files(*.*)|*.*"
        opendialog.FileName = ""
        opendialog.DefaultExt = ".tre"
        opendialog.Multiselect = False
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            tree_path = opendialog.FileName
            Me.Activate()
            Disable_Windows()
            CloseDataToolStripMenuItem.Enabled = True
            tree_path = opendialog.FileName
            If tree_path.ToLower.EndsWith(".t") Then
                mrbayes_tree = True
            Else
                mrbayes_tree = False
            End If
            error_no = load_names(opendialog.FileName.Substring(opendialog.FileName.LastIndexOf(".") + 1))
            error_no = load_trees()
            If error_no = 0 Then
                Dim make_Tree As New Thread(AddressOf load_final_trees)
                make_Tree.CurrentCulture = ci
                make_Tree.Start()
            End If

        End If
    End Sub

    Private Sub SaveResultToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveResultToolStripMenuItem.Click
        If File.Exists(root_path + "temp" + path_char + "analysis_result.log") Then
            Dim opendialog As New SaveFileDialog
            opendialog.Filter = "Text File (*.txt)|*.txt;*.TXT|ALL Files(*.*)|*.*"
            opendialog.FileName = ""
            opendialog.DefaultExt = ".txt"
            opendialog.CheckFileExists = False
            opendialog.CheckPathExists = True
            Dim resultdialog As DialogResult = opendialog.ShowDialog()
            If resultdialog = DialogResult.OK Then
                File.Copy(root_path + "temp" + path_char + "analysis_result.log", opendialog.FileName, True)
                Me.Invoke(RT1, New Object() {"Save Successfully!" + vbCrLf})
            End If
        Else
            MsgBox("No result to save!", MsgBoxStyle.Information)
        End If
    End Sub

    Private Sub SaveLogToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveLogToolStripMenuItem.Click
        Dim opendialog As New SaveFileDialog
        opendialog.Filter = "Log File (*.log)|*.log;*.LOG|ALL Files(*.*)|*.*"
        opendialog.FileName = ""
        opendialog.DefaultExt = ".log"
        opendialog.CheckFileExists = False
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            CmdBox.SaveFile(opendialog.FileName, RichTextBoxStreamType.UnicodePlainText)
            Me.Invoke(RT1, New Object() {"Save Successfully!" + vbCrLf})
        End If
    End Sub
    Private Sub SaveDistributionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveDistributionToolStripMenuItem.Click
        Dim opendialog As New SaveFileDialog
        opendialog.Filter = "CSV File (*.csv)|*.csv;*.CSV|Phylip File (*.txt)|*.txt;*.TXT|ALL Files(*.*)|*.*"
        opendialog.FileName = ""
        opendialog.DefaultExt = ".csv"
        opendialog.CheckFileExists = False
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            If opendialog.FileName.ToLower.EndsWith(".csv") Then
                Dim dw As New StreamWriter(opendialog.FileName, False)
                Dim state_line As String = "ID,Name"
                For j As Integer = 3 To DataGridView1.ColumnCount - 1
                    state_line += "," + DataGridView1.Columns(j).HeaderText
                Next
                dw.WriteLine(state_line)
                For i As Integer = 1 To dtView.Count
                    If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "False" Then
                        state_line = i.ToString
                        For j As Integer = 2 To DataGridView1.ColumnCount - 1
                            state_line += "," + dtView.Item(i - 1).Item(j - 1)
                        Next
                        dw.WriteLine(state_line)
                    End If
                Next
                dw.Close()
            Else
                Dim dw As New StreamWriter(opendialog.FileName, False)
                RangeStr = ""
                For i As Integer = 1 To dtView.Count
                    For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                        If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                            If RangeStr.Contains(c) = False Then
                                RangeStr = RangeStr + c.ToString
                            End If
                        Else
                            MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                            Exit Sub
                        End If
                    Next
                Next
                For Each c As Char In RangeStr.ToUpper
                    If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                        MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                        Exit Sub
                    End If
                Next

                If RangeStr.Length = 1 Then
                    MsgBox("There should be two different areas at least!")
                    Exit Sub
                End If
                dw.WriteLine(dtView.Count.ToString + "	" + RangeStr.Length.ToString)
                For i As Integer = 1 To dtView.Count
                    dw.WriteLine(dtView.Item(i - 1).Item(1) + "	" + Distributiton_to_Binary(dtView.Item(i - 1).Item(state_index).ToString, RangeStr.Length))
                Next
                dw.Close()
            End If
            Me.Invoke(RT1, New Object() {"Save Successfully!" + vbCrLf})
        End If
    End Sub


    Private Sub OpenResultToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenResultToolStripMenuItem.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Text File (*.txt)|*.txt;*.TXT|ALL Files(*.*)|*.*"
        opendialog.FileName = ""
        opendialog.DefaultExt = ".txt"
        opendialog.CheckFileExists = False
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            If File.Exists(opendialog.FileName) Then
                StartTreeView = True
                File.Copy(opendialog.FileName, root_path + "temp" + path_char + "analysis_result.log", True)
                Dim CopyFileInfo As New FileInfo(root_path + "temp" + path_char + "analysis_result.log")
                CopyFileInfo.Attributes = FileAttributes.Normal
                tree_view_title = opendialog.FileName
                Dim Tree_view_form As New View_Tree
                Tree_view_form.Show()
            End If
        End If
    End Sub

    Public Sub read_new_bayes_log()
        Me.Invoke(RT2_S, New Object() {"Second setp ..."})
        Dim waitforbayes As Boolean = False
        Do
            Try
                If File.Exists(root_path + "temp" + path_char + "clade1.nex.mcmc") Then
                    File.Delete(root_path + "temp" + path_char + "clade1.nex.mcmc")
                    waitforbayes = False
                End If
                If File.Exists(root_path + "temp" + path_char + "clade1.nex.run1.t") Then
                    File.Delete(root_path + "temp" + path_char + "clade1.nex.run1.t")
                    waitforbayes = False
                End If
                If File.Exists(root_path + "temp" + path_char + "clade1.nex.run2.t") Then
                    File.Delete(root_path + "temp" + path_char + "clade1.nex.run2.t")
                    waitforbayes = False
                End If
                If File.Exists(root_path + "temp" + path_char + "analysis_result.log") Then
                    File.Delete(root_path + "temp" + path_char + "analysis_result.log")
                    waitforbayes = False
                End If
                waitforbayes = True
            Catch ex As Exception
                System.Threading.Thread.Sleep(512)
            End Try
        Loop Until waitforbayes
        Dim sr As New StreamReader(root_path + "temp" + path_char + "clade_b.log")
        Dim line As String
        line = sr.ReadLine
        Dim swf As StreamWriter

        swf = New StreamWriter(root_path + "temp" + path_char + "analysis_result.log", True)
        swf.WriteLine("Bayesian Analysis result file of " + state_header)
        swf.WriteLine("[TAXON]")
        For i As Integer = 1 To dtView.Count
            swf.WriteLine(dtView.Item(i - 1).Item(0).ToString + "	" + dtView.Item(i - 1).Item(1).ToString + "	" + dtView.Item(i - 1).Item(state_index).ToString.ToUpper)
        Next
        swf.WriteLine("[TREE]")
        swf.WriteLine("Tree=" + tree_show_with_value)
        swf.WriteLine("[RESULT]")
        Dim result_r1() As String
        Dim result_r2() As String
        Dim result_f() As String
        Dim node_number As Integer = tree_show_with_value.Length - tree_show_with_value.Replace("(", "").Length
        ReDim result_r1(node_number - 1)
        ReDim result_r2(node_number - 1)
        ReDim result_f(node_number - 1)
        ReDim num_r1(0)
        ReDim num_r2(0)
        Dim Temp_array As Integer = 0
        Process_Text += vbCrLf + "Result of run 1:" + vbCrLf
        Do
            Dim p_list() As String = line.Split(New Char() {"="c})(1).Split(New Char() {"	"c})
            Dim new_list() As String
            ReDim new_list(RangeStr.Length * 2)
            Dim temp_arr As Integer = 0
            For i As Integer = 1 To RangeStr.Length - 1
                For j As Integer = i + 1 To RangeStr.Length
                    new_list(i * 2 - 1) = Val(new_list(i * 2 - 1)) + Val(p_list(temp_arr * 4 + 1)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 3)) / (RangeStr.Length - 1)
                    new_list(i * 2) = Val(new_list(i * 2)) + Val(p_list(temp_arr * 4 + 2)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 4)) / (RangeStr.Length - 1)
                    new_list(j * 2 - 1) = Val(new_list(j * 2 - 1)) + Val(p_list(temp_arr * 4 + 1)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 2)) / (RangeStr.Length - 1)
                    new_list(j * 2) = Val(new_list(j * 2)) + Val(p_list(temp_arr * 4 + 3)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 4)) / (RangeStr.Length - 1)
                    temp_arr += 1
                Next
            Next

            For Each i As String In new_list
                If i <> "" Then
                    num_r1(UBound(num_r1)) = Val(i)
                    ReDim Preserve num_r1(UBound(num_r1) + 1)
                End If
            Next


            result_r1(Temp_array) = make_range_P(RangeStr.Length, new_list, BayesForm.NumericUpDown1.Value, line.Split(New Char() {"="c})(0).Replace(" ", "") + ".r")
            Temp_array += 1
            line = sr.ReadLine
        Loop Until line.StartsWith("-")
        Temp_array = 0
        line = sr.ReadLine
        Process_Text += vbCrLf + "Result of run 2:" + vbCrLf
        Do
            Dim p_list() As String = line.Split(New Char() {"="c})(1).Split(New Char() {"	"c})
            Dim new_list() As String
            ReDim new_list(RangeStr.Length * 2)
            Dim temp_arr As Integer = 0
            For i As Integer = 1 To RangeStr.Length - 1
                For j As Integer = i + 1 To RangeStr.Length
                    new_list(i * 2 - 1) = Val(new_list(i * 2 - 1)) + Val(p_list(temp_arr * 4 + 1)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 3)) / (RangeStr.Length - 1)
                    new_list(i * 2) = Val(new_list(i * 2)) + Val(p_list(temp_arr * 4 + 2)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 4)) / (RangeStr.Length - 1)
                    new_list(j * 2 - 1) = Val(new_list(j * 2 - 1)) + Val(p_list(temp_arr * 4 + 1)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 2)) / (RangeStr.Length - 1)
                    new_list(j * 2) = Val(new_list(j * 2)) + Val(p_list(temp_arr * 4 + 3)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 4)) / (RangeStr.Length - 1)
                    temp_arr += 1
                Next
            Next

            For Each i As String In new_list
                If i <> "" Then
                    num_r2(UBound(num_r2)) = Val(i)
                    ReDim Preserve num_r2(UBound(num_r2) + 1)
                End If
            Next
            result_r2(Temp_array) = make_range_P(RangeStr.Length, new_list, BayesForm.NumericUpDown1.Value, line.Split(New Char() {"="c})(0).Replace(" ", "") + ".r")
            Temp_array += 1
            line = sr.ReadLine
        Loop Until line.StartsWith("-")
        Temp_array = 0
        line = sr.ReadLine
        Dim distance As Single = 0
        For i As Integer = 0 To UBound(num_r1) - 1
            distance = distance + (num_r1(i) - num_r2(i)) ^ 2 ^ 0.5 / 2
        Next
        distance = distance / (UBound(num_r1) - 1)
        Process_Text += vbCrLf + "distance of run 1 and run 2: " + distance.ToString("F4") + vbCrLf
        Process_Text += vbCrLf + "Result of combined:" + vbCrLf
        Do
            Dim p_list() As String = line.Split(New Char() {"="c})(1).Split(New Char() {"	"c})
            Dim new_list() As String
            ReDim new_list(RangeStr.Length * 2)
            Dim temp_arr As Integer = 0
            For i As Integer = 1 To RangeStr.Length - 1
                For j As Integer = i + 1 To RangeStr.Length
                    new_list(i * 2 - 1) = Val(new_list(i * 2 - 1)) + Val(p_list(temp_arr * 4 + 1)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 3)) / (RangeStr.Length - 1)
                    new_list(i * 2) = Val(new_list(i * 2)) + Val(p_list(temp_arr * 4 + 2)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 4)) / (RangeStr.Length - 1)
                    new_list(j * 2 - 1) = Val(new_list(j * 2 - 1)) + Val(p_list(temp_arr * 4 + 1)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 2)) / (RangeStr.Length - 1)
                    new_list(j * 2) = Val(new_list(j * 2)) + Val(p_list(temp_arr * 4 + 3)) / (RangeStr.Length - 1) + Val(p_list(temp_arr * 4 + 4)) / (RangeStr.Length - 1)
                    temp_arr += 1
                Next
            Next
            result_f(Temp_array) = make_range_P(RangeStr.Length, new_list, BayesForm.NumericUpDown1.Value, line.Split(New Char() {"="c})(0).Replace(" ", "") + ".r")
            Temp_array += 1
            line = sr.ReadLine
        Loop Until line.StartsWith("-")
        Temp_array = 0
        swf.WriteLine("Result of combined:")

        For Each i As String In result_f
            swf.WriteLine(i)
        Next
        swf.WriteLine("Result of run 1:")

        For Each i As String In result_r1
            swf.WriteLine(i)
        Next
        swf.WriteLine("Result of run 2:")
        For Each i As String In result_r2
            swf.WriteLine(i)
        Next

        swf.WriteLine("[PROBABILITY]")
        sr.Close()
        Dim st As New StreamReader(root_path + "temp" + path_char + "clade_b.log")
        Dim temp_log As String
        Do
            temp_log = st.ReadLine
        Loop Until temp_log.StartsWith("-")
        Do
            temp_log = st.ReadLine
        Loop Until temp_log.StartsWith("-")
        temp_log = "	"
        For i As Integer = 1 To RangeStr.Length - 1
            For j As Integer = i + 1 To RangeStr.Length
                temp_log = temp_log + Chr(i + 64).ToString + Chr(j + 64).ToString + "(A)	" + Chr(i + 64).ToString + Chr(j + 64).ToString + "(C)	" + Chr(i + 64).ToString + Chr(j + 64).ToString + "(G)	" + Chr(i + 64).ToString + Chr(j + 64).ToString + "(T)	"
            Next
        Next
        swf.WriteLine(temp_log)
        temp_log = st.ReadLine
        Do
            If temp_log.StartsWith("-") = False Then
                Dim cur_clade As Integer = CInt(temp_log.Split(New Char() {" "c})(0).ToLower.Replace("clade", "").Replace("_p", ""))
                Dim result As String = "node " + (cur_clade + taxon_num).ToString
                temp_log = result + ": " + temp_log.Split(New Char() {"="c})(1)
                swf.WriteLine(temp_log)
            End If
            temp_log = st.ReadLine
        Loop Until temp_log = ""
        st.Close()
        swf.WriteLine("[END]")
        swf.Close()
        bayesIsrun = False
        Dim isclear As Boolean = True
        Do
            Try
                isclear = True
                If File.Exists(root_path + "temp" + path_char + "clade1.nex") Then
                    isclear = False
                    File.Delete(root_path + "temp" + path_char + "clade1.nex")
                End If
            Catch ex As Exception
                System.Threading.Thread.Sleep(500)
            End Try
        Loop Until isclear
        Process_Text += vbCrLf + "Analysis end at " + Date.Now.ToString + vbCrLf
        tree_view_title = "Bayesian Binary MCMC Analysis Result"
        Me.Invoke(RT2_S, New Object() {""})
        StartTreeView = True
        Process_ID = -1
    End Sub
    Public Function make_new_range(ByVal range_num As Integer, ByVal Single_P1() As String, ByVal Max_area As Single, ByVal ID As String) As String
        Dim NewCom As New Module_PerCom
        Dim Temp_range() As String
        Dim ComArray As Object
        Dim range_p(0) As Single
        Dim range_str(0) As String
        Dim rang_array As Integer = 0
        Dim rang_sum As Single = 0
        ReDim Temp_range(RangeStr.Length - 1)
        For i As Integer = 0 To RangeStr.Length - 1
            Temp_range(i) = Chr(65 + i)
        Next
        For j As Integer = 1 To Max_area
            ComArray = NewCom.Combine(Temp_range, j)


            ReDim Preserve range_p(UBound(ComArray) + range_p.Length)
            ReDim Preserve range_str(UBound(range_p))
            For i As Integer = 0 To UBound(ComArray)
                Dim line() As Char = Distributiton_to_Binary(ComArray(i), RangeStr.Length)
                Dim Temp_p As Single = 1
                For k As Integer = 0 To UBound(line)
                    If line(k) = "0" Then
                        Temp_p = Temp_p * (Val(Single_P1(k * 2 + 1)))
                    Else
                        Temp_p = Temp_p * (Val(Single_P1(k * 2 + 2)))
                    End If
                Next
                rang_sum = rang_sum + Temp_p
                range_p(rang_array) = Temp_p
                range_str(rang_array) = ComArray(i)
                rang_array += 1
            Next
        Next
        If BayesForm.CheckBox1.Checked Then
            ReDim Preserve range_p(range_p.Length)
            ReDim Preserve range_str(UBound(range_p))
            Dim line() As Char = Distributiton_to_Binary("/", RangeStr.Length)
            Dim Temp_p As Single = 1
            For k As Integer = 0 To UBound(line)
                Temp_p = Temp_p * (Val(Single_P1(k * 2 + 1)))
            Next
            rang_sum = rang_sum + Temp_p
            range_p(rang_array) = Temp_p
            range_str(rang_array) = "/"
        End If
        Array.Sort(range_p, range_str, New scomparer)
        Dim cur_clade As Integer = CInt(ID.Split(New Char() {"."c})(0).ToLower.Replace("clade", "").Replace("_p", ""))
        Dim t_list() As String = Poly_Node(cur_clade - 1, 3).Split(New Char() {","c})
        Dim result As String = "node " + (cur_clade + taxon_num).ToString + " (anc. of terminals " + t_list(0) + "-" + t_list(UBound(t_list)) + "):"
        If rang_sum > 0 Then
            Dim sw As New StreamWriter(root_path + "temp" + path_char + ID)
            sw.WriteLine(result)
            Process_Text += "node " + (cur_clade + taxon_num).ToString
            Process_Text += " (terminals " + t_list(0) + "-" + t_list(UBound(t_list)) + "):"
            For i As Integer = 0 To UBound(range_p) - 1
                If i < 3 Or range_p(i) / rang_sum * 100 >= 5 Then
                    Process_Text += " " + range_str(i) + " " + (range_p(i) / rang_sum * 100).ToString("F2") + "%"
                End If
                result = result + " " + range_str(i) + " " + (range_p(i) / rang_sum * 100).ToString("F2")
                sw.WriteLine(range_str(i) + "	" + (range_p(i) / rang_sum * 100).ToString("F2"))
            Next
            Process_Text += " " + vbCrLf
            sw.Close()
        End If
        Return result
    End Function

    Private Sub LagrangeAnalysisToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)


    End Sub
    Public Sub loadDEC()
        lag_gen = 0
        Process_ID = 5
        current_dir = Directory.GetCurrentDirectory
        Directory.SetCurrentDirectory(root_path)
        runlag("./temp/final.lg", "temp/lg_result.txt", lag_gen)

        Dim wr3 As New StreamWriter(root_path + "temp\SDEC_0.bat", False, System.Text.Encoding.Default)

        wr3.WriteLine("""" + root_path + "Plug-ins\Lagrange_Win.exe" + """" + " final.lg>result.1.dec")
        wr3.WriteLine("exit")
        wr3.Close()

        current_dir = Directory.GetCurrentDirectory
        Directory.SetCurrentDirectory(root_path + "temp\")

        Dim startInfo As New ProcessStartInfo
        startInfo.FileName = "SDEC_0.bat"
        startInfo.WorkingDirectory = root_path + "temp"
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = Lagrange_Config.CheckBox1.Checked
        startInfo.RedirectStandardOutput = Lagrange_Config.CheckBox1.Checked
        startInfo.RedirectStandardInput = Lagrange_Config.CheckBox1.Checked
        startInfo.RedirectStandardError = Lagrange_Config.CheckBox1.Checked
        startInfo.WindowStyle = ProcessWindowStyle.Minimized
        Process.Start(startInfo)


        Me.Invoke(RT2_S, New Object() {"Run S-DEC ..."})
        Directory.SetCurrentDirectory(current_dir)

    End Sub
    Private Sub Lag_Timer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Lag_Timer.Tick
        If Lag_con_made Then
            Lag_con_made = False
            Me.Invoke(RT1, New Object() {vbCrLf + "*******************************************" + vbCrLf})
            If dec_mode = 0 Then
                Me.Invoke(RT1, New Object() {"*DEC Analysis*" + vbCrLf})
            Else
                Me.Invoke(RT1, New Object() {"*S-DEC Analysis*" + vbCrLf})
            End If
            Me.Invoke(RT1, New Object() {"*******************************************" + vbCrLf})
            Me.Invoke(RT1, New Object() {"Process begin at " + Date.Now.ToString + vbCrLf})
            If dec_mode = 0 Then
                Dim lb As New Thread(AddressOf loadDEC)
                lb.CurrentCulture = ci
                lb.Start()
            Else
                CheckForIllegalCrossThreadCalls = False
                Dim lb As New Thread(AddressOf make_file_dec)
                lb.CurrentCulture = ci
                lb.Start()
            End If
            Disable_Windows()
            Lag_Timer.Enabled = False
        End If
    End Sub
    'Dim SBGB_count As Integer
    Public Sub make_file_BGB()
        Dim current_tree As String
        Dim rt As New StreamReader(root_path + "temp" + path_char + "clean_num.trees")
        Dim burn_in As Integer = CInt(BurninBox.Text)
        Do While burn_in > 0
            rt.ReadLine()
            burn_in = burn_in - 1
        Loop
        Dim seed As Integer = DateTime.Now.Millisecond
        If Global_seed <> "20180127" Then
            seed = Global_seed
        End If
        Dim rand As New System.Random(seed)
        Dim is_random As Boolean = False
        Dim random_num As Integer = 0
        Dim random_array(0) As Integer
        Try
            Process_Int = 0
            Process_ID = 1
            Dim tree_num As Integer = 1
            Me.Invoke(RT2_S, New Object() {"Making command ..."})

            Dim sr0 As New StreamReader(root_path + "Plug-ins\BGB\header.r")
            Dim BGB_header As String = sr0.ReadToEnd
            sr0.Close()
            Dim sr1 As New StreamReader(root_path + "temp\BGB_model.r")
            Dim BGB_model = sr1.ReadToEnd
            sr1.Close()

            Dim temp_BGB As String = BGB_header.Replace("#lib_path#", lib_path)
            If rscript = root_path + "Plug-ins\R\bin\i386\Rscript.exe" Then
                temp_BGB = temp_BGB.Replace("#r_lib#", "")
            End If
            If rscript = root_path + "Plug-ins\R\bin\x64\Rscript.exe" Then
                temp_BGB = temp_BGB.Replace("#r_lib#", "")
            End If
            Dim sr2 As New StreamReader(root_path + "Plug-ins\BGB\footer.r")
            Dim BGB_footer As String = sr2.ReadToEnd
            sr2.Close()
            Dim mutiple_BGB() As String
            ReDim mutiple_BGB(muti_threads_BGB)
            For i As Integer = 0 To muti_threads_BGB - 1
                Dim sw_header As New StreamWriter(root_path + "temp\LOAD_SBGB_" + i.ToString + ".r", False)
                sw_header.Write(temp_BGB)
                sw_header.WriteLine("source(" + """" + "SBGB_" + i.ToString + ".r" + """" + ")")
                sw_header.WriteLine("})")
                sw_header.Close()
                mutiple_BGB(i) = "tryCatch({" + vbCrLf
            Next
            Dim mutiple_threads_id As Integer = 0
            Dim Analyse_trees As New StreamWriter(root_path + "temp" + path_char + "random_trees.tre", False)
            For t As Integer = CInt(BurninBox.Text) + 1 To CInt(TreeBox.Text)
                mutiple_threads_id = t Mod muti_threads_BGB
                If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
                    Dim random_w As New StreamWriter(root_path + "temp" + path_char + "random_num.txt", False)
                    ReDim random_array(CInt(RandomTextBox.Text))
                    random_w.WriteLine(RandomTextBox.Text)
                    is_random = True
r2:                 If random_num < CInt(RandomTextBox.Text) Then
                        random_num = random_num + 1
                        t = rand.Next(CInt(BurninBox.Text) + 1, CInt(TreeBox.Text))
                        mutiple_threads_id = t Mod muti_threads_BGB
                        random_array(random_num) = t
                        random_w.WriteLine(t.ToString)
                        GoTo n_r1
                    End If
                    random_w.Close()
                    Exit For
                End If
n_r1:           If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
                    Dim random_rt As New StreamReader(root_path + "temp" + path_char + "clean_num.trees")
                    For i As Integer = 1 To t - 1
                        random_rt.ReadLine()
                    Next
                    current_tree = random_rt.ReadLine()
                    random_rt.Close()
                Else
                    current_tree = rt.ReadLine()
                End If
                Analyse_trees.WriteLine(current_tree)
                If File.Exists(root_path + "temp" + path_char + t.ToString + ".txt") Then
                    File.Delete(root_path + "temp" + path_char + t.ToString + ".txt")
                End If

                Dim savepath As String = (root_path + "temp" + path_char + "temp_BGB").Replace("\", "/")
                current_tree = current_tree.Replace(";", "")
                If current_tree.Contains(":") = False Then
                    Me.Invoke(RT1, New Object() {"Warning: Your trees dataset do not contain branch length!" + vbCrLf})
                    Me.Invoke(RT1, New Object() {"Try to add branch length to trees with [Tools -> Add Branch Length]." + vbCrLf})
                    Me.Invoke(RT2_S, New Object() {""})
                    current_tree = current_tree.Replace(")", "):0.0001")
                End If
                '把没有支长的分支加上0.01，从而兼容BGB
                current_tree = current_tree.Replace("):", "#$:#")
                current_tree = current_tree.Replace(")", "):0.0001")
                current_tree = current_tree.Replace("#$:#", "):")

                Dim NumofTaxon As Integer = current_tree.Length - current_tree.Replace(",", "").Length + 1
                Dim NumofNode As Integer = current_tree.Length - current_tree.Replace("(", "").Length
                get_tree_length(current_tree)
                Dim root_time As Single = config_tree_time / maxtime
                Dim wr As New StreamWriter(savepath + "." + tree_num.ToString + ".tre", False)
                Dim temp As String = ""
                For i As Integer = 1 To Tree_Export_Char.Length - 1
                    If Tree_Export_Char(i).Contains(":") Then
                        If Tree_Export_Char(i - 1) <> ")" Then
                            temp += dtView.Item(CInt(Tree_Export_Char(i).Split(New Char() {":"c})(0)) - 1).Item(1).ToString + ":" + (Max(Val(Tree_Export_Char(i).Split(New Char() {":"c})(1)), 0.0001) * root_time).ToString("F8")
                        Else
                            temp += Val(Tree_Export_Char(i).Split(New Char() {":"c})(0)).ToString + ":" + (Max(Val(Tree_Export_Char(i).Split(New Char() {":"c})(1)), 0.0001) * root_time).ToString("F8")
                        End If
                    Else
                        temp += Tree_Export_Char(i)
                    End If
                Next
                temp += ";"
                wr.WriteLine(temp)
                wr.Close()
                Dim wr1 As New StreamWriter(savepath + "." + tree_num.ToString + ".data", False)
                Dim temp_range As String = ""
                For j As Integer = 0 To dtView.Count - 1
                    For Each c As Char In dtView.Item(j).Item(state_index).ToString.ToUpper
                        If temp_range.Contains(c) = False Then
                            temp_range += c
                        End If
                    Next
                Next
                Dim area_labels As String = ""
                Dim temp_areas() As Char = RangeStr.ToUpper
                Array.Sort(temp_areas)
                For Each i As Char In temp_areas
                    area_labels += i.ToString + " "
                Next
                temp = NumofTaxon.ToString + "	" + temp_range.Length.ToString + " (" + area_labels.Remove(area_labels.Length - 1) + ")" + vbCrLf
                For i As Integer = 1 To NumofTaxon
                    temp = temp + vbCrLf + dtView.Item(i - 1).Item(1).ToString + "	" + Distributiton_to_Binary(dtView.Item(i - 1).Item(state_index).ToString.ToUpper, temp_range.Length)
                Next
                wr1.Write(temp)
                wr1.Close()

                If is_random Then
                    PV_SUM = CInt(RandomTextBox.Text)
                    Process_Int = CInt(random_num / PV_SUM * 10000)
                    mutiple_BGB(mutiple_threads_id) += vbCrLf + "write(" + """" + Process_Int.ToString + """" + ",file=" + """" + "BGB.state" + """" + ")" + vbCrLf
                    mutiple_BGB(mutiple_threads_id) += BGB_model.Replace("#treefile#", savepath + "." + tree_num.ToString + ".tre").Replace("#datafile#", savepath + "." + tree_num.ToString + ".data").Replace("#model_tab#", savepath + "." + tree_num.ToString + ".tab")
                    tree_num += 1
                    GoTo r2
                End If
                PV_SUM = (CInt(TreeBox.Text) - CInt(BurninBox.Text))
                Process_Int = CInt((t - CInt(BurninBox.Text)) / PV_SUM * 10000)
                mutiple_BGB(mutiple_threads_id) += vbCrLf + "write(" + """" + Process_Int.ToString + """" + ",file=" + """" + "BGB.state" + """" + ")" + vbCrLf
                mutiple_BGB(mutiple_threads_id) += BGB_model.Replace("#treefile#", savepath + "." + tree_num.ToString + ".tre").Replace("#datafile#", savepath + "." + tree_num.ToString + ".data").Replace("#model_tab#", savepath + "." + tree_num.ToString + ".tab")
                tree_num += 1
            Next

            For i As Integer = 0 To muti_threads_BGB - 1
                Dim wr2 As New StreamWriter(root_path + "temp\SBGB_" + i.ToString + ".r", True)
                wr2.Write(mutiple_BGB(i))
                wr2.WriteLine(BGB_footer.Replace("BGB.end", "SBGB_" + i.ToString + ".end"))
                wr2.Close()
            Next

            For i As Integer = 0 To muti_threads_BGB - 1
                Dim wr3 As New StreamWriter(root_path + "temp\SBGB_" + i.ToString + ".bat", False, System.Text.Encoding.Default)
                wr3.WriteLine("""" + rscript + """" + " LOAD_SBGB_" + i.ToString + ".r>SBGB_" + i.ToString + ".log")
                wr3.WriteLine("echo end>SBGB_" + i.ToString + ".end")
                wr3.WriteLine("exit")
                wr3.Close()
            Next

            current_dir = Directory.GetCurrentDirectory
            Directory.SetCurrentDirectory(root_path + "temp\")
            For i As Integer = 0 To muti_threads_BGB - 1
                Dim startInfo As New ProcessStartInfo
                startInfo.FileName = "SBGB_" + i.ToString + ".bat"
                startInfo.WorkingDirectory = root_path + "temp"
                startInfo.UseShellExecute = False
                startInfo.CreateNoWindow = BGB_Config.CheckBox1.Checked
                startInfo.RedirectStandardOutput = BGB_Config.CheckBox1.Checked
                startInfo.RedirectStandardInput = BGB_Config.CheckBox1.Checked
                startInfo.RedirectStandardError = BGB_Config.CheckBox1.Checked
                startInfo.WindowStyle = ProcessWindowStyle.Minimized
                Process.Start(startInfo)
            Next
            Directory.SetCurrentDirectory(current_dir)

            BGB_gen = 0
            Process_Int = 0
            Process_ID = 10

            SBGB_count = tree_num - 1
            Analyse_trees.Close()
            Me.Invoke(RT2_S, New Object() {"BioGeoBEARS package is loading..."})
            CheckForIllegalCrossThreadCalls = True
        Catch ex As Exception
            MsgBox(ex.ToString)
            MsgBox("Cannot process the trees!")
            Exit Sub
        End Try
        rt.Close()
    End Sub

    Public Sub make_file_dec()
        Dim current_tree As String
        Dim rt As New StreamReader(root_path + "temp" + path_char + "clean_num.trees")
        Dim burn_in As Integer = CInt(BurninBox.Text)
        Do While burn_in > 0
            rt.ReadLine()
            burn_in = burn_in - 1
        Loop
        Dim seed As Integer = DateTime.Now.Millisecond
        If Global_seed <> "20180127" Then
            seed = Global_seed
        End If
        Dim rand As New System.Random(seed)
        Dim is_random As Boolean = False
        Dim random_num As Integer = 0
        Dim random_array(0) As Integer
        Try
            Process_Int = 0
            Process_ID = 1
            Dim tree_num As Integer = 1
            Me.Invoke(RT2_S, New Object() {"Making command ..."})
            Dim Analyse_trees As New StreamWriter(root_path + "temp" + path_char + "random_trees.tre", False)

            Dim mutiple_DEC() As String
            ReDim mutiple_DEC(muti_threads_DEC)
            For i As Integer = 0 To muti_threads_DEC - 1
                mutiple_DEC(i) = ""
            Next
            Dim mutiple_threads_id As Integer = 0

            For t As Integer = CInt(BurninBox.Text) + 1 To CInt(TreeBox.Text)

                If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
                    Dim random_w As New StreamWriter(root_path + "temp" + path_char + "random_num.txt", False)
                    ReDim random_array(CInt(RandomTextBox.Text))
                    random_w.WriteLine(RandomTextBox.Text)
                    is_random = True
r2:                 If random_num < CInt(RandomTextBox.Text) Then
                        random_num = random_num + 1
                        t = rand.Next(CInt(BurninBox.Text) + 1, CInt(TreeBox.Text))
                        random_array(random_num) = t
                        random_w.WriteLine(t.ToString)
                        GoTo n_r1
                    End If
                    random_w.Close()
                    Exit For
                End If
n_r1:           If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
                    Dim random_rt As New StreamReader(root_path + "temp" + path_char + "clean_num.trees")
                    For i As Integer = 1 To t - 1
                        random_rt.ReadLine()
                    Next
                    current_tree = random_rt.ReadLine()
                    random_rt.Close()
                Else
                    current_tree = rt.ReadLine()
                End If
                Analyse_trees.WriteLine(current_tree)
                If File.Exists(root_path + "temp" + path_char + t.ToString + ".txt") Then
                    File.Delete(root_path + "temp" + path_char + t.ToString + ".txt")
                End If

                Dim savepath As String = root_path + "temp" + path_char + "temp_lg"
                current_tree = current_tree.Replace(";", "")
                If current_tree.Contains(":") = False Then
                    MsgBox("Your trees dataset do not contain branch length! ")
                    Me.Invoke(RT2_S, New Object() {""})
                    Process_Text = ""
                    Process_Int = 0
                    Process_ID = -1
                    Exit Sub
                End If
                Dim NumofTaxon As Integer = current_tree.Length - current_tree.Replace(",", "").Length + 1
                Dim NumofNode As Integer = current_tree.Length - current_tree.Replace("(", "").Length
                get_tree_length(current_tree)
                Dim root_time As Single = config_tree_time / maxtime
                Dim wr As New StreamWriter(savepath + "." + tree_num.ToString + ".tre", False)
                Dim temp As String = ""
                For i As Integer = 1 To Tree_Export_Char.Length - 1
                    If Tree_Export_Char(i).Contains(":") Then
                        If Tree_Export_Char(i - 1) <> ")" Then
                            temp += dtView.Item(CInt(Tree_Export_Char(i).Split(New Char() {":"c})(0)) - 1).Item(1).ToString + ":" + (Val(Tree_Export_Char(i).Split(New Char() {":"c})(1)) * root_time).ToString("F8")
                        Else
                            temp += Val(Tree_Export_Char(i).Split(New Char() {":"c})(0)).ToString + ":" + (Val(Tree_Export_Char(i).Split(New Char() {":"c})(1)) * root_time).ToString("F8")
                        End If
                    Else
                        temp += Tree_Export_Char(i)
                    End If
                Next
                temp += ";"
                wr.WriteLine(temp)
                wr.Close()
                Dim wr1 As New StreamWriter(savepath + "." + tree_num.ToString + ".data", False)
                Dim temp_range As String = ""
                For j As Integer = 0 To dtView.Count - 1
                    For Each c As Char In dtView.Item(j).Item(state_index).ToString.ToUpper
                        If temp_range.Contains(c) = False Then
                            temp_range += c
                        End If
                    Next
                Next
                temp = NumofTaxon.ToString + "	" + temp_range.Length.ToString
                For i As Integer = 1 To NumofTaxon
                    temp = temp + vbCrLf + dtView.Item(i - 1).Item(1).ToString + "	" + Distributiton_to_Binary(dtView.Item(i - 1).Item(state_index).ToString.ToUpper, temp_range.Length)
                Next
                wr1.Write(temp)
                wr1.Close()
                Dim temp_lg As String = sdec_lg
                temp_lg = sdec_lg.Replace("#treefile#", savepath + "." + tree_num.ToString + ".tre")
                temp_lg = temp_lg.Replace("#datafile#", savepath + "." + tree_num.ToString + ".data")
                File.Copy(root_path + "temp\final.rm", savepath + "." + tree_num.ToString + ".rm")
                temp_lg = temp_lg.Replace("#ratematrix#", savepath + "." + tree_num.ToString + ".rm")
                Dim wr2 As New StreamWriter(savepath + "." + tree_num.ToString + ".lg", False)
                wr2.Write(temp_lg)
                wr2.Close()
                Dim wr3 As New StreamWriter(savepath + "." + tree_num.ToString + ".tree", False)
                wr3.WriteLine(current_tree)
                wr3.Close()
                tree_num += 1
                If is_random Then
                    PV_SUM = CInt(RandomTextBox.Text)
                    Process_Int = CInt(random_num / PV_SUM * 10000)
                    GoTo r2
                End If
                PV_SUM = (CInt(TreeBox.Text) - CInt(BurninBox.Text))
                Process_Int = CInt((t - CInt(BurninBox.Text)) / PV_SUM * 10000)
            Next
            Process_Int = 0
            Process_ID = 6
            lag_gen = -1
            sdec_id = tree_num - 1
            sdec_count = tree_num - 1
            Analyse_trees.Close()


            For i As Integer = 0 To muti_threads_DEC - 1
                Dim wr3 As New StreamWriter(root_path + "temp\SDEC_" + i.ToString + ".bat", False, System.Text.Encoding.Default)
                For t As Integer = 1 To sdec_count
                    If t Mod muti_threads_DEC = i Then
                        wr3.WriteLine("""" + root_path + "Plug-ins\Lagrange_Win.exe" + """" + " temp_lg." + t.ToString + ".lg>result." + t.ToString + ".dec")
                    End If
                Next
                wr3.WriteLine("echo end>SDEC_" + i.ToString + ".end")
                wr3.WriteLine("exit")
                wr3.Close()
            Next

            current_dir = Directory.GetCurrentDirectory
            Directory.SetCurrentDirectory(root_path + "temp\")
            For i As Integer = 0 To muti_threads_DEC - 1
                Dim startInfo As New ProcessStartInfo
                startInfo.FileName = "SDEC_" + i.ToString + ".bat"
                startInfo.WorkingDirectory = root_path + "temp"
                startInfo.UseShellExecute = False
                startInfo.CreateNoWindow = Lagrange_Config.CheckBox1.Checked
                startInfo.RedirectStandardOutput = Lagrange_Config.CheckBox1.Checked
                startInfo.RedirectStandardInput = Lagrange_Config.CheckBox1.Checked
                startInfo.RedirectStandardError = Lagrange_Config.CheckBox1.Checked
                startInfo.WindowStyle = ProcessWindowStyle.Minimized
                Process.Start(startInfo)
            Next
            Directory.SetCurrentDirectory(current_dir)

            Me.Invoke(RT2_S, New Object() {"Run S-DEC ..."})
            CheckForIllegalCrossThreadCalls = True
        Catch ex As Exception
            MsgBox(ex.ToString)
            MsgBox("Cannot process the trees!")
            Exit Sub
        End Try
        rt.Close()
    End Sub
    Dim TaxonTime(,) As String
    Dim Tree_Export_Char() As String
    Public Sub get_tree_length(ByVal Treeline As String)

        Dim NumofNode As Integer = Treeline.Length - Treeline.Replace(")", "").Length
        Dim NumofTaxon As Integer = Treeline.Length - Treeline.Replace(",", "").Length + 1
        Dim Poly_Node_row(,) As Single
        Dim Poly_terminal_xy(,) As Single
        Dim Poly_Node_col(,) As Single
        Dim taxon_array() As String
        ReDim Poly_Node(NumofNode - 1, 9) '0 root,1 末端, 2 子节点, 3 全部链, 4 左侧个数, 5 右侧个数, 6 支持率,7 枝长, 8 总长, 9 祖先节点
        ReDim Poly_Node_row(NumofNode - 1, 2)
        ReDim Poly_Node_col(NumofNode - 1, 2)
        ReDim Poly_terminal_xy(NumofTaxon - 1, 2)
        ReDim TaxonTime(NumofTaxon - 1, 2)
        ReDim taxon_array(NumofTaxon - 1)
        For i As Integer = 0 To NumofNode - 1
            Poly_Node(i, 0) = 0
            Poly_Node(i, 1) = ""
            Poly_Node(i, 2) = ""
            Poly_Node(i, 3) = ""
            Poly_Node(i, 6) = "1.00"
            Poly_Node(i, 7) = "0"
            Poly_Node(i, 8) = "0"
        Next
        ReDim tree_char(NumofTaxon * 7)
        Dim char_id As Integer = 0
        Dim l_c As Integer = 0
        Dim r_c As Integer = 0
        Dim tx As Integer = 0
        Dim last_symb As Boolean = True
        For Each i As Char In Treeline
            Select Case i
                Case "("
                    char_id += 1
                    tree_char(char_id) = i
                    last_symb = True

                Case ")"
                    char_id += 1
                    tree_char(char_id) = i
                    last_symb = True
                Case ","
                    char_id += 1
                    tree_char(char_id) = i
                    last_symb = True
                Case Else
                    If last_symb Then
                        char_id += 1
                        tree_char(char_id) = i
                        last_symb = False
                    Else
                        tree_char(char_id) += i
                    End If
            End Select
        Next
        ReDim Tree_Export_Char(char_id)
        For i As Integer = 1 To char_id
            Tree_Export_Char(i) = tree_char(i)
        Next

        For i As Integer = 1 To char_id
            If Tree_Export_Char(i).Contains(":") Then
                If Tree_Export_Char(i - 1) <> ")" Then
                    '物种
                    TaxonTime(CInt(tree_char(i).Split(New Char() {":"c})(0)) - 1, 0) = tree_char(i).Split(New Char() {":"c})(1)
                    tree_char(i) = tree_char(i).Split(New Char() {":"c})(0)
                End If
            End If
        Next

        Dim point_1, point_2 As Integer
        point_1 = 0
        point_2 = 0
        Dim Temp_node(,) As String
        ReDim Temp_node(NumofNode, 6) '0 节点位置,1 末端, 2 子节点, 4 左侧个数, 5 右侧个数, 6 支持率
        For i As Integer = 0 To NumofNode - 1
            Temp_node(i, 0) = ""
            Temp_node(i, 1) = ""
            Temp_node(i, 2) = ""
            Temp_node(i, 4) = "32768"
            Temp_node(i, 5) = "0"
            Temp_node(i, 6) = "1"
        Next
        For i As Integer = 1 To char_id
            Select Case tree_char(i)
                Case "("
                    l_c += 1
                    Temp_node(point_1, 0) = i
                    point_1 += 1
                Case ")"
                    r_c += 1
                    Poly_Node(point_2, 1) = Temp_node(point_1 - 1, 1)
                    Poly_Node(point_2, 2) = Temp_node(point_1 - 1, 2)
                    Poly_Node(point_2, 4) = Temp_node(point_1 - 1, 4)
                    Poly_Node(point_2, 5) = Temp_node(point_1 - 1, 5)
                    For j As Integer = Temp_node(point_1 - 1, 0) To i
                        If tree_char(j) <> "(" And tree_char(j) <> ")" Then
                            If tree_char(j) <> "," Then
                                If tree_char(j - 1) <> ")" Then
                                    Poly_Node(point_2, 3) += tree_char(j)
                                End If
                            Else
                                Poly_Node(point_2, 3) += tree_char(j)
                            End If
                        End If
                    Next
                    If point_1 > 1 Then
                        Temp_node(point_1 - 2, 2) = point_2.ToString + "," + Temp_node(point_1 - 2, 2)
                        Temp_node(point_1 - 2, 4) = Min(Val(Temp_node(point_1 - 2, 4)), (Val(Poly_Node(point_2, 5)) + Val(Poly_Node(point_2, 4))) / 2)
                        Temp_node(point_1 - 2, 5) = Max(Val(Temp_node(point_1 - 2, 5)), (Val(Poly_Node(point_2, 5)) + Val(Poly_Node(point_2, 4))) / 2)
                    End If
                    point_2 += 1
                    point_1 -= 1
                    Temp_node(point_1, 0) = ""
                    Temp_node(point_1, 1) = ""
                    Temp_node(point_1, 2) = ""
                    Temp_node(point_1, 4) = "32768"
                    Temp_node(point_1, 5) = "0"
                Case ","
                Case Else
                    If tree_char(i - 1) = ")" Then
                        '读取支持率
                        If tree_char(i).Contains(":") Then
                            If Val(tree_char(i).Split(New Char() {":"c})(0)) > 1 Then
                                Poly_Node(point_2 - 1, 6) = (Val(tree_char(i)) / 100).ToString("F2")
                            Else
                                Poly_Node(point_2 - 1, 6) = Val(tree_char(i)).ToString("F2")
                            End If
                            Poly_Node(point_2 - 1, 7) = tree_char(i).Split(New Char() {":"c})(1)
                        Else
                            If Val(tree_char(i)) > 1 Then
                                Poly_Node(point_2 - 1, 6) = (Val(tree_char(i)) / 100).ToString("F2")
                            Else
                                Poly_Node(point_2 - 1, 6) = Val(tree_char(i)).ToString("F2")
                            End If
                        End If

                    Else
                        taxon_array(tx) = tree_char(i)
                        tx += 1
                        Temp_node(point_1 - 1, 1) += tree_char(i) + ","
                        Temp_node(point_1 - 1, 4) = Min(Val(Temp_node(point_1 - 1, 4)), tx)
                        Temp_node(point_1 - 1, 5) = Max(Val(Temp_node(point_1 - 1, 4)), tx)
                    End If
            End Select
        Next
        make_chain(NumofNode - 1)
        maxtime = 0
        For i As Integer = 0 To NumofTaxon - 1
            If maxtime < Val(TaxonTime(i, 1)) Then
                maxtime = Val(TaxonTime(i, 1))
            End If
        Next
        If Write_Tree_Info Then
            Me.Invoke(SetTI, New Object() {"Length of Tree: " + maxtime.ToString("F2") + vbCrLf})
        End If
    End Sub
    Dim maxtime As Single
    Public Sub make_chain(ByVal n As Integer)
        If Poly_Node(n, 2) <> "" Then
            Dim anc_node() As String = Poly_Node(n, 2).Split(New Char() {","c})
            For Each j As String In anc_node
                If j <> "" Then
                    Poly_Node(CInt(j), 8) = (Val(Poly_Node(CInt(j), 7)) + Val(Poly_Node(n, 8))).ToString
                    make_chain(CInt(j))
                End If
            Next
        End If
        If Poly_Node(n, 1) <> "" Then
            Dim anc_node() As String = Poly_Node(n, 1).Split(New Char() {","c})
            For Each j As String In anc_node
                If j <> "" Then
                    TaxonTime(CInt(j) - 1, 1) = (Val(TaxonTime(CInt(j) - 1, 0)) + Val(Poly_Node(n, 8))).ToString
                End If
            Next
        End If
    End Sub
    Public Function CleanLine(ByVal inputline As String) As String
        If inputline Is Nothing = False Then
            inputline = inputline.Replace("	", "")
            Do
                inputline = inputline.Replace("  ", " ")
            Loop Until inputline.Length = inputline.Replace("  ", " ").Length
            Do While inputline.StartsWith(" ") = True
                inputline = inputline.Remove(0, 1)
            Loop
        End If
        Return inputline
    End Function
    Public Function Readnex(ByVal nex_file As String) As Integer
        Try
            Array.Clear(CharMatrix, 0, taxon_num - 1)
            Array.Clear(TaxonList, 0, taxon_num - 1)
            Dim sr As New StreamReader(nex_file)
            Dim line As String = ""
            Dim TaxonNum As Integer
            Do
                line = CleanLine(line)
                '读取数据矩阵
                If line.ToUpper.StartsWith("MATRIX") Then
                    Do
                        line = sr.ReadLine
                        line = CleanLine(line)
                        If line <> "" And line.StartsWith("[") = False And line.StartsWith(";") = False And line.ToUpper.StartsWith("END") = False Then
                            Dim Taxon As String, charac As String, Num As Integer
                            Taxon = line.Substring(0, line.IndexOf(" "))
                            charac = line.Substring(line.IndexOf(" ") + 1, line.Length - line.IndexOf(" ") - 1)
                            If Array.IndexOf(TaxonList, Taxon) < 0 Then
                                Num = Num + 1
                                TaxonNum = Num
                                TaxonList(TaxonNum - 1) = Taxon
                                CharMatrix(TaxonNum - 1) = charac
                            Else
                                Num = Num + 1
                                If Num Mod TaxonNum <> 0 Then
                                    CharMatrix((Num Mod TaxonNum) - 1) = CharMatrix((Num Mod TaxonNum) - 1) + charac.Replace(" ", "")
                                Else
                                    CharMatrix(TaxonNum - 1) = CharMatrix(TaxonNum - 1) + charac.Replace(" ", "")
                                End If
                            End If
                        End If
                    Loop Until line.ToUpper.StartsWith("END")
                    'replace "."
                    If CharMatrix(0).IndexOf(".") < 0 And CharMatrix(1).IndexOf(".") >= 0 Then
                        'MsgBox("Paup GUI will replace '.' to the state of first Taxon !")
                        For j As Integer = 1 To TaxonNum - 1
                            For k As Integer = 0 To CharMatrix(0).Length - 1
                                If CharMatrix(j).Chars(k) = "." Then
                                    CharMatrix(j) = CharMatrix(j).Remove(k, 1)
                                    CharMatrix(j) = CharMatrix(j).Insert(k, CharMatrix(0).Chars(k))
                                End If
                            Next
                        Next
                    End If
                End If
                line = sr.ReadLine
            Loop Until line Is Nothing
            sr.Close()
            Return 0
        Catch ex As Exception
            Return 1
        End Try
    End Function
    Public Function Readphy(ByVal file_path As String) As Integer
        Try
            Array.Clear(CharMatrix, 0, taxon_num - 1)

            Array.Clear(TaxonList, 0, taxon_num - 1)
            Dim sr As New StreamReader(file_path)
            Dim line As String = sr.ReadLine
            Dim matrix_length As Double = CDbl(line.Split(" ")(1))
            For i As Integer = 0 To taxon_num - 1
                line = sr.ReadLine
                CharMatrix(i) = line.Split("	")(1).Substring(0, matrix_length)
                TaxonList(i) = line.Split("	")(0)
            Next
            sr.Close()
            Return 0
        Catch ex As Exception
            Return 1
        End Try

    End Function
    Private Sub DPP_Timer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DPP_Timer.Tick
        If dpp_gen <> -1 And dpp_gen <> -2 Then
            Process_ID = -2
            Try
                Me.Invoke(PV, New Object() {CInt(dpp_gen)})
            Catch ex As Exception

            End Try

        Else
            Select Case dpp_gen
                Case -1
                    Me.Invoke(PV, New Object() {0})
                    DPP_Timer.Enabled = False
                    Process_ID = -1
                    Dim opendialog As New SaveFileDialog
                    opendialog.Filter = "Tree File (*.tre)|*.tre;*.TRE|ALL Files(*.*)|*.*"
                    opendialog.FileName = ""
                    opendialog.DefaultExt = ".tre"
                    opendialog.CheckFileExists = False
                    opendialog.CheckPathExists = True
                    Dim resultdialog As DialogResult = opendialog.ShowDialog()
                    If resultdialog = DialogResult.OK Then
                        File.Copy(root_path + "temp" + path_char + "dppresult.format.tre", opendialog.FileName, True)
                        Me.Invoke(RT1, New Object() {"Save Successfully!" + vbCrLf})
                    End If
            End Select

        End If
    End Sub

    Private Sub BayesianAnalysisOfBiogeographyBayareaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Public Function Right_to_Left(ByVal right_id As Integer, ByVal tree_line As String) As Integer
        Dim t_num As Integer = tree_line.Length - tree_line.Replace(",", "").Length + 1
        right_id = right_id - t_num
        Dim t1 As Integer = 0
        Dim t2 As Integer = 0
        Dim t3 As Integer = 0
        Dim t_c As Integer = 0
        For i As Integer = 0 To tree_line.Length - 1
            If tree_line.Chars(i) = ")" Then
                t1 += 1
            End If
            If tree_line.Chars(i) = "(" Then
                t2 += 1
            End If
            t_c += 1
            If t1 = right_id Then
                Exit For
            End If
        Next
        For i As Integer = t_c - 1 To 0 Step -1
            If tree_line.Chars(i) = ")" Then
                t3 += 1
            End If
            If tree_line.Chars(i) = "(" Then
                t3 -= 1
                t2 -= 1
            End If
            If t3 = 0 Then
                Exit For
            End If
        Next
        Return t_num + t2 + 1
    End Function


    Private Sub LoadDataMatrixToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadDataToolStripMenuItem.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Phylip Files (*.*)|*.*"
        opendialog.FileName = ""
        opendialog.DefaultExt = "*.*"
        opendialog.CheckFileExists = False
        opendialog.CheckPathExists = True
        Dim TaxonName() As String
        Dim resultdialog As DialogResult = opendialog.ShowDialog()

        If resultdialog = DialogResult.OK Then
            ReDim TaxonName(taxon_num - 1)
            For i As Integer = 0 To taxon_num - 1
                TaxonName(i) = DataGridView1.Rows(i).Cells(2).Value
            Next
            ReDim CharMatrix(taxon_num - 1)
            ReDim TaxonList(taxon_num - 1)
            Dim sr As New StreamReader(opendialog.FileName)
            Dim first_line As String = sr.ReadLine
            sr.Close()
            Dim read_file As Integer = 0
            If first_line.ToUpper.StartsWith("#NEXUS") Then
                read_file = Readnex(opendialog.FileName)
            Else
                read_file = Readphy(opendialog.FileName)
            End If
            If read_file = 1 Then
                MsgBox("Could not load data! Please use NEXUS or PHYLIP format!")
                Exit Sub
            End If
            Dim wr As New StreamWriter(root_path + "temp" + path_char + "dppdiv.dat")
            wr.WriteLine(taxon_num.ToString + " " + CharMatrix(0).Length.ToString)
            For i As Integer = 0 To taxon_num - 1
                Dim temp_num As Integer = Array.IndexOf(TaxonName, TaxonList(i))
                If temp_num < 0 Then
                    MsgBox("Could not find " + TaxonList(i) + " in your data file!")
                    Exit Sub
                End If
                wr.WriteLine((temp_num + 1).ToString + "	" + CharMatrix(i))
            Next
            wr.Close()
            CmdBox.AppendText("Load Data Matrix Successfully!" + vbCrLf)
            DPPAnalysisToolStripMenuItem.Enabled = True
        End If
    End Sub

    Private Sub DirichletProcessPriorAnalysisToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DPPAnalysisToolStripMenuItem.Click
        If first_open(6) Then
            If isDebug = False Then
                MsgBox("This method described in:" + Chr(13) + "Heath, Holder, Huelsenbeck. 2012. A Dirichlet process prior for estimating lineage-specific substitution rates. Molecular Biology and Evolution 29:939-955")
            End If
            first_open(6) = False
        End If
        DataGridView1.EndEdit()
        ProgressBar1.Maximum = 1000

        If CInt(TreeBox.Text) > 0 Then

            If final_tree.Replace(",", "").Length <> final_tree.Replace("(", "").Length Then
                MsgBox("Condensed tree contains " + (final_tree.Replace("(", "").Length - final_tree.Replace(",", "").Length).ToString + " polytomies!")
                Exit Sub
            End If
            DPPdiv_Config.ComboBox1.Items.Clear()
            DPPdiv_Config.ComboBox1.Items.Add("root")
            For i As Integer = taxon_num + 1 To DIVAForm.ComboBox1.Items.Count
                DPPdiv_Config.ComboBox1.Items.Add(DIVAForm.ComboBox1.Items(i - 1).ToString.Replace("node", "clade"))
            Next
            DPPdiv_Config.Visible = True
        Else
            MsgBox("Need at least one tree!")
        End If
    End Sub

    Private Sub TracerViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracerViewToolStripMenuItem.Click
        TracerForm.Show()
    End Sub

    Private Sub CombineResultsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CombineResultsToolStripMenuItem.Click
        CombineForm.Show()
    End Sub

    Private Sub RandomTreesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RandomTreesToolStripMenuItem.Click
        If File.Exists(root_path + "temp" + path_char + "clean_num_p.trees") Then
            Dim opendialog As New SaveFileDialog
            opendialog.Filter = "Tree File (*.trees)|*.trees;*.TREES|ALL Files(*.*)|*.*"
            opendialog.FileName = ""
            opendialog.DefaultExt = ".trees"
            opendialog.CheckFileExists = False
            opendialog.CheckPathExists = True

            Dim resultdialog As DialogResult = opendialog.ShowDialog()
            If resultdialog = DialogResult.OK Then
                export_rand_tree = opendialog.FileName
                Process_ID = 2
                Dim th1 As New Thread(AddressOf make_rand_tree)
                th1.Start()
            End If
        Else
            MsgBox("RASP is formating trees. Please try again later")
        End If
    End Sub
    Dim export_rand_tree As String
    Public Sub make_rand_tree()

        Dim seed As Integer = DateTime.Now.Millisecond
        If Global_seed <> "20180127" Then
            seed = Global_seed
        End If
        Dim rand As New System.Random(seed)
        Dim t As Integer = 0
        Dim wr As New StreamWriter(export_rand_tree, False)
        wr.WriteLine("#NEXUS")
        wr.WriteLine("Begin trees;")
        wr.WriteLine("   Translate")
        For i As Integer = 1 To dtView.Count - 1
            wr.WriteLine(dtView.Item(i - 1).Item(0).ToString + " " + dtView.Item(i - 1).Item(1).ToString + ",")
        Next
        wr.WriteLine(dtView.Item(dtView.Count - 1).Item(0).ToString + " " + dtView.Item(dtView.Count - 1).Item(1).ToString)
        wr.WriteLine("		;")
        'Dim count As Integer = 1
        Dim ran_list(0) As Integer
        ReDim ran_list(CInt(MainWindow.RandomTextBox.Text))
        ran_list(0) = 0
        For i As Integer = 1 To CInt(MainWindow.RandomTextBox.Text)
            t = rand.Next(CInt(MainWindow.BurninBox.Text) + 1, CInt(MainWindow.TreeBox.Text))
            ran_list(i) = t
        Next

        Array.Sort(ran_list)
        Dim sr As New StreamReader(root_path + "temp" + path_char + "clean_num_p.trees")
        Dim line As String = ""
        For i As Integer = 1 To CInt(MainWindow.RandomTextBox.Text)
            For j As Integer = ran_list(i - 1) + 1 To ran_list(i)
                line = sr.ReadLine()
            Next
            If line <> "" Then
                Me.Invoke(PV, New Object() {CInt(10000 * i / (CInt(RandomTextBox.Text) + 1))})
                wr.WriteLine("tree random" + i.ToString + " = " + line)
            End If
        Next
        sr.Close()

        wr.WriteLine("End;")
        wr.Close()
        Me.Invoke(PV, New Object() {0})
        Process_ID = -1
        Me.Invoke(RT1, New Object() {"Save Successfully!" + vbCrLf})
    End Sub
    Private Function remove_taxon(ByVal tree_line As String, ByVal taxon_id As Integer) As String
        Dim new_tree As String = ""
        Dim NumofTaxon As Integer = tree_line.Length - tree_line.Replace(",", "").Length + 1
        Dim old_tree_char() As String
        Dim tree_poly() As Char = tree_line
        ReDim old_tree_char(NumofTaxon * 10)
        Dim char_id As Integer = 0
        Dim last_symb As Boolean = True
        For i As Integer = 0 To tree_poly.Length - 1
            Select Case tree_poly(i)
                Case "(", ")", ":", ",", ";"
                    char_id += 1
                    old_tree_char(char_id) = tree_poly(i)
                    last_symb = True
                Case Else
                    If last_symb Then
                        char_id += 1
                        old_tree_char(char_id) = tree_poly(i)
                        last_symb = False
                    Else
                        old_tree_char(char_id) += tree_poly(i)
                    End If
            End Select
        Next
        Dim tax_pos As Integer = -1
        Dim left_pos As Integer = 0
        Dim right_pos As Integer = 0
        Do
            tax_pos += 1
            tax_pos = Array.IndexOf(old_tree_char, taxon_id.ToString, tax_pos)
        Loop Until old_tree_char(tax_pos - 1) <> ":" And old_tree_char(tax_pos - 1) <> ")"
        Select Case old_tree_char(tax_pos - 1)
            Case "("
                left_pos = tax_pos - 1
                Dim temp_count As Integer = 1
                For i As Integer = tax_pos To char_id
                    If old_tree_char(i) = "(" Then
                        temp_count += 1 
                    End If
                    If old_tree_char(i) = ")" Then
                        temp_count -= 1
                    End If
                    If temp_count = 0 Then
                        right_pos = i
                        Exit For
                    End If
                Next
                Dim add_length As Single = 0
                Select Case old_tree_char(right_pos + 1)
                    Case ";", ",", ")"
                    Case ":"
                        add_length = Val(old_tree_char(right_pos + 2))
                        old_tree_char(right_pos + 1) = ""
                        old_tree_char(right_pos + 2) = ""
                    Case Else
                        If old_tree_char(right_pos + 2) = ":" Then
                            add_length = Val(old_tree_char(right_pos + 3))
                            old_tree_char(right_pos + 1) = ""
                            old_tree_char(right_pos + 2) = ""
                            old_tree_char(right_pos + 3) = ""
                        Else
                            'MsgBox("What is this? " + old_tree_char(right_pos + 1))
                        End If
                End Select
                If add_length > 0 Then
                    If old_tree_char(right_pos - 2) = ":" Then
                        old_tree_char(right_pos - 1) = Val(old_tree_char(right_pos - 1)) + add_length
                    End If
                End If
                Select Case old_tree_char(tax_pos + 1)
                    Case ","
                        old_tree_char(right_pos) = ""
                        old_tree_char(left_pos) = ""
                        old_tree_char(tax_pos - 1) = ""
                        old_tree_char(tax_pos) = ""
                        old_tree_char(tax_pos + 1) = ""
                    Case ":"
                        old_tree_char(right_pos) = ""
                        old_tree_char(left_pos) = ""
                        old_tree_char(tax_pos - 1) = ""
                        old_tree_char(tax_pos) = ""
                        old_tree_char(tax_pos + 1) = ""
                        old_tree_char(tax_pos + 2) = ""
                        old_tree_char(tax_pos + 3) = ""
                    Case Else
                        MsgBox("Error: You are using unrooted trees or trees with polytomies.")
                End Select
            Case ","
                Dim temp_count As Integer = 1
                For i As Integer = tax_pos To 0 Step -1
                    If old_tree_char(i) = "(" Then
                        temp_count -= 1
                    End If
                    If old_tree_char(i) = ")" Then
                        temp_count += 1
                    End If
                    If temp_count = 0 Then
                        left_pos = i
                        Exit For
                    End If
                Next
                For i As Integer = tax_pos To char_id
                    If old_tree_char(i) = ")" Then
                        right_pos = i
                        Exit For
                    End If
                Next
                Dim add_length As Single = 0
                Select Case old_tree_char(right_pos + 1)
                    Case ";", ",", ")"
                    Case ":"
                        add_length = Val(old_tree_char(right_pos + 2))
                        old_tree_char(right_pos + 1) = ""
                        old_tree_char(right_pos + 2) = ""
                    Case Else
                        If old_tree_char(right_pos + 2) = ":" Then
                            add_length = Val(old_tree_char(right_pos + 3))
                            old_tree_char(right_pos + 1) = ""
                            old_tree_char(right_pos + 2) = ""
                            old_tree_char(right_pos + 3) = ""
                        ElseIf old_tree_char(right_pos + 2) = ";" Then
                            old_tree_char(right_pos + 1) = ""
                        Else
                            MsgBox("What is this? " + old_tree_char(right_pos + 1))
                        End If
                End Select
                If add_length > 0 Then
                    If old_tree_char(tax_pos - 3) = ":" Then
                        old_tree_char(tax_pos - 2) = Val(old_tree_char(tax_pos - 2)) + add_length
                    End If
                End If
                Select Case old_tree_char(tax_pos + 1)
                    Case ")"
                        old_tree_char(right_pos) = ""
                        old_tree_char(left_pos) = ""
                        old_tree_char(tax_pos - 1) = ""
                        old_tree_char(tax_pos) = ""
                        old_tree_char(tax_pos + 1) = ""
                    Case ":"
                        old_tree_char(right_pos) = ""
                        old_tree_char(left_pos) = ""
                        old_tree_char(tax_pos - 1) = ""
                        old_tree_char(tax_pos) = ""
                        old_tree_char(tax_pos + 1) = ""
                        old_tree_char(tax_pos + 2) = ""
                    Case Else
                        MsgBox("What is this? " + old_tree_char(tax_pos + 1))
                End Select
            Case Else
                MsgBox("What is this? " + old_tree_char(tax_pos - 1))
        End Select
        For Each i As String In old_tree_char
            new_tree += i
        Next
        Return new_tree
    End Function

    Private Sub DebugToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DebugToolStripMenuItem.Click

        RangeStr = "ab"
        read_geiger(root_path + "test.txt", root_path + "test.tre", "", "ER", False)
        Dim sw As New StreamWriter(root_path + "test_result.txt")
        sw.Write(rasp_result)
        sw.Close()
        'Dim x As Object = InputBox("Input", "Debug")
        'MsgBox(Left_to_right(x, final_tree))
        'Shell("cmd")
        'RangeStr = "ABCD"
        'taxon_num = 19
        'read_BGB(root_path + "tabDEC.txt", root_path + "final.tre")
        'MsgBox(check_r_package({"gensa", "snow", "phylobase", "biogeobears"}, True))
        'ctree(27, root_path, cons_tre)
    End Sub

    Private Sub RemoveOutgroupToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveOutgroupToolStripMenuItem.Click
        DataGridView1.EndEdit()
        If CInt(TreeBox_P.Text) > 0 Then
            Dim og As String = ""
            For i As Integer = 1 To dtView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    og = i.ToString
                End If
            Next
            If og = "" Then
                MsgBox("Please select one outgroup at least!")
                Exit Sub
            End If
            Dim opendialog As New SaveFileDialog
            opendialog.Filter = "ALL Files(*.*)|*.*"
            opendialog.FileName = ""
            opendialog.CheckFileExists = False
            opendialog.CheckPathExists = True
            Dim resultdialog As DialogResult = opendialog.ShowDialog()

            If resultdialog = DialogResult.OK Then
                file_remove_og = opendialog.FileName
                Process_ID = 8
            End If
        Else
            MsgBox("No trees found!")
        End If

    End Sub
    Dim file_remove_og As String
    Public Sub write_remove_og()
        Dim new_id() As String
        ReDim new_id(dtView.Count)
        Dim wr As New StreamWriter(file_remove_og + "_treeset.trees", False)
        Dim dw As New StreamWriter(file_remove_og + "_dis.csv", False)

        wr.WriteLine("#NEXUS")
        wr.WriteLine("Begin trees;")
        wr.WriteLine("   Translate")
        Dim temp_count As Integer = 0
        Dim new_count As Integer = 0
        For i As Integer = 1 To dtView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "False" Then
                new_count += 1
            End If
        Next
        Dim state_line As String = "ID,Name"
        For j As Integer = 3 To DataGridView1.ColumnCount - 1
            state_line += "," + DataGridView1.Columns(j).HeaderText
        Next
        dw.WriteLine(state_line)
        For i As Integer = 1 To dtView.Count
            If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "False" Then
                state_line = i.ToString
                For j As Integer = 2 To DataGridView1.ColumnCount - 1
                    state_line += "," + dtView.Item(i - 1).Item(j - 1)
                Next
                dw.WriteLine(state_line)
                temp_count += 1
                new_id(i) = temp_count
                If temp_count <> new_count Then
                    wr.WriteLine(temp_count.ToString + " " + dtView.Item(i - 1).Item(1).ToString + ",")
                Else
                    wr.WriteLine(temp_count.ToString + " " + dtView.Item(i - 1).Item(1).ToString)
                End If
            Else
                new_id(i) = ""
            End If
        Next
        dw.Close()
        wr.WriteLine("		;")
        Dim sr As New StreamReader(root_path + "temp" + path_char + "clean_num_p.trees", True)
        Dim line As String = sr.ReadLine
        Dim tree_count As Integer = 1
        Do
            For i As Integer = 1 To dtView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    line = remove_taxon(line, i)
                End If
            Next
            Dim has_length As Boolean = line.Contains(":")
            If has_length Then
                For i As Integer = 1 To dtView.Count
                    If new_id(i) <> "" Then
                        line = line.Replace("(" + i.ToString + ":", "($%*" + new_id(i) + "$%*:")
                        line = line.Replace("," + i.ToString + ":", ",$%*" + new_id(i) + "$%*:")
                        line = line.Replace("," + i.ToString + ":", ",$%*" + new_id(i) + "$%*:")
                    End If
                Next
            End If
            For i As Integer = 1 To dtView.Count
                If new_id(i) <> "" Then
                    line = line.Replace("(" + i.ToString + ",", "($%*" + new_id(i) + "$%*,")
                    line = line.Replace("," + i.ToString + ")", ",$%*" + new_id(i) + "$%*)")
                    line = line.Replace("," + i.ToString + ",", ",$%*" + new_id(i) + "$%*,")
                End If
            Next
            line = line.Replace("$%*", "")
            wr.WriteLine("tree new_tree" + tree_count.ToString + " = " + line)
            Process_Int = Int(tree_count / CInt(TreeBox_P.Text) * 10000)
            tree_count += 1
            line = sr.ReadLine
        Loop Until line Is Nothing
        sr.Close()
        wr.WriteLine("End;")
        wr.Close()

        If FinalTreeBox.Text <> "" Then
            Dim wrf As New StreamWriter(file_remove_og + "_tree.tre", False)
            wrf.WriteLine("#NEXUS")
            wrf.WriteLine("Begin trees;")
            wrf.WriteLine("   Translate")
            temp_count = 0
            For i As Integer = 1 To dtView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "False" Then
                    temp_count += 1
                    new_id(i) = temp_count
                    If temp_count <> new_count Then
                        wrf.WriteLine(temp_count.ToString + " " + dtView.Item(i - 1).Item(1).ToString + ",")
                    Else
                        wrf.WriteLine(temp_count.ToString + " " + dtView.Item(i - 1).Item(1).ToString)
                    End If
                Else
                    new_id(i) = ""
                End If
            Next
            wrf.WriteLine("		;")
            line = FinalTreeBox.Text
            For i As Integer = 1 To dtView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    line = remove_taxon(line, i)
                End If
            Next
            Dim has_length As Boolean = line.Contains(":")
            If has_length Then
                For i As Integer = 1 To dtView.Count
                    If new_id(i) <> "" Then
                        line = line.Replace("(" + i.ToString + ":", "($%*" + new_id(i) + "$%*:")
                        line = line.Replace("," + i.ToString + ":", ",$%*" + new_id(i) + "$%*:")
                        line = line.Replace("," + i.ToString + ":", ",$%*" + new_id(i) + "$%*:")
                    End If
                Next
            End If
            For i As Integer = 1 To dtView.Count
                If new_id(i) <> "" Then
                    line = line.Replace("(" + i.ToString + ",", "($%*" + new_id(i) + "$%*,")
                    line = line.Replace("," + i.ToString + ")", ",$%*" + new_id(i) + "$%*)")
                    line = line.Replace("," + i.ToString + ",", ",$%*" + new_id(i) + "$%*,")
                End If
            Next
            line = line.Replace("$%*", "")
            wrf.WriteLine("tree con_tree" + " = " + line)
            wrf.WriteLine("End;")
            wrf.Close()
        End If
        Process_Text = vbCrLf + "Export successfully!" + vbCrLf + "Please close the current data and load your saved one."
        Process_ID = -1
        CheckForIllegalCrossThreadCalls = True
    End Sub

    Public Sub set_style(ByVal set_string As String, ByVal style_id As Integer)
        TreeInfo.AppendText(set_string)
        TreeInfo.Select(Max(0, TreeInfo.TextLength - set_string.Length), set_string.Length)
        Select Case style_id
            Case 0
                Dim Table_font As Font = New Font("Tahoma", 10, FontStyle.Bold)
                TreeInfo.SelectionFont = Table_font
            Case 1
                TreeInfo.SelectionColor = Color.Blue
            Case 2
                TreeInfo.SelectionColor = Color.Red
            Case 3
                TreeInfo.SelectionColor = Color.Green
            Case 4
                TreeInfo.Text = set_string
                TreeInfo.Select(0, set_string.Length)
                Dim Table_font As Font = New Font("Tahoma", 10, FontStyle.Bold)
                TreeInfo.SelectionFont = Table_font
        End Select
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim enable_bb As Boolean = True
        Dim enable_ba As Boolean = True
        Dim enable_sd As Boolean = True
        Dim enable_dec As Boolean = True
        Dim enable_s_dec As Boolean = True
        Dim enable_bt As Boolean = True
        Dim enable_bear As Boolean = True
        Dim enable_s_bear As Boolean = True
        Dim enable_bayestraits As Boolean = True
        If FinalTreeBox.Text <> "" Then
            Write_Tree_Info = True
            TreeInfo.Text = ""
            set_style("CHECKING CONDENSED TREE:" + vbCrLf, 0)
            get_tree_length(FinalTreeBox.Text.Substring(0, FinalTreeBox.Text.Length - 1))

            Dim NumofTaxon As Integer = 1 + FinalTreeBox.Text.Length - FinalTreeBox.Text.Replace(",", "").Length
            Dim NumofNode As Integer = FinalTreeBox.Text.Length - FinalTreeBox.Text.Replace("(", "").Length
            TreeInfo.AppendText("Number of Taxon: " + NumofTaxon.ToString + vbCrLf)
            TreeInfo.AppendText("Number of Node: " + NumofNode.ToString + vbCrLf)
            Dim sr As New StreamReader(root_path + "temp" + path_char + "clean_num.trees")
            Dim line As String = sr.ReadLine
            sr.Close()
            If line = "" Then
                Dim sr1 As New StreamReader(root_path + "temp" + path_char + "v_tree2.tre")
                line = sr1.ReadLine
                sr1.Close()
            End If
            Dim branch_count As Integer = line.Length - line.Replace(":", "").Length
            If branch_count = 0 Then
                set_style("WARNING: S-DEC need trees with branch length." + vbCrLf, 1)
                set_style("WARNING: BioGeoBEARS need trees with branch length." + vbCrLf, 1)
                set_style("WARNING: BayesTraits need trees with branch length." + vbCrLf, 1)
                enable_dec = False
                enable_bear = False
                enable_s_bear = False
                enable_bayestraits = False
            End If
            If NumofTaxon > 126 Then
                set_style("WARNING: S-DIVA may stop when there are too many taxons" + vbCrLf, 1)
                set_style("WARNING: DEC, S-DEC and BioGeoBEARS may take extremely long time to complete the analysis." + vbCrLf, 1)
            End If
            If FinalTreeBox.Text.Contains(":") = False Then
                set_style("WARNING: DEC, BayArea, Bayestraits and BioGeoBEARS need a condensed tree with branch length." + vbCrLf, 1)
                enable_ba = False
                enable_dec = False
                enable_bear = False
                enable_s_bear = False
                enable_bayestraits = False

            End If
            Read_Poly_Node(FinalTreeBox.Text)
            Dim root_time As Single = 0
            For i As Integer = 0 To taxon_num - 1
                If root_time < Val(TaxonTime(i, 1)) Then
                    root_time = Val(TaxonTime(i, 1))
                End If

            Next
            If FinalTreeBox.Text.Contains(":-") Then
                set_style("WARNING: BioGeoBEARS need a condensed tree with branch length greater than zero." + vbCrLf, 1)
                set_style("INFO: Try [Tools->Scaling Branch Length] to optimize your tree." + vbCrLf, 3)
                enable_bear = False
            End If
            set_style("INFO: The total length of the final tree is: " + root_time.ToString("F8") + vbCrLf, 3)
            If root_time < 1 Then
                set_style("WARNING: The statistical methods of BioGeoBEARS may meet error when the branch length of tree is too small." + vbCrLf, 1)
                set_style("INFO: Try [Tools->Scaling Branch Length] if you meet errors." + vbCrLf, 3)
            End If
            If NumofTaxon > 512 Then
                enable_sd = False
                set_style("WARNING: S-DIVA could not analysis more than 512 taxon" + vbCrLf, 1)
            End If
            If NumofNode > 512 Then
                set_style("WARNING: BBM may not analysis more than 512 nodes" + vbCrLf, 1)
                enable_bb = False
            End If
            If FinalTreeBox.Text.Replace(",", "").Length - FinalTreeBox.Text.Replace("(", "").Length = 0 Then
                TreeInfo.AppendText("Condensed Tree: No polytomies" + vbCrLf)
            Else
                TreeInfo.AppendText("Condensed Tree: " + (FinalTreeBox.Text.Replace("(", "").Length - FinalTreeBox.Text.Replace(",", "").Length).ToString + " Polytomies" + vbCrLf)
                If mrbayes_tree Then
                    set_style("INFO: Root the trees from Mrbayes" + vbCrLf, 3)
                Else
                    set_style("WARNING:DEC and BioGeoBEARS need a binary condensed tree" + vbCrLf, 1)
                    enable_dec = False
                    enable_bear = False
                End If
            End If

            If StatisticalMethodsToolStripMenuItem.Enabled = False Then
                enable_s_dec = False
                set_style("WARNING: RASP is still making S-DEC trees dataset now. Please wait some minutes and try again." + vbCrLf, 1)
            End If
            set_style("CHECKING TREES DATASET:" + vbCrLf, 0)
            If CheckBox3.Checked Then
                TreeInfo.AppendText("Random trees: Enabled " + "(" + RandomTextBox.Text + " trees)" + vbCrLf)
            Else
                TreeInfo.AppendText("Random trees: Disabled" + vbCrLf)
                set_style("INFO: Use Random trees for S-DIVA and S-DEC if there are too many trees" + vbCrLf, 3)
            End If
            If CInt(TreeBox.Text) < 10 Then
                set_style("WARNING: too few trees for S-DIVA and S-DEC" + vbCrLf, 1)
                If CInt(TreeBox.Text) < 2 Then
                    enable_s_dec = False
                    set_style("WARNING: S-DEC need at least two binary trees" + vbCrLf, 1)
                    If CInt(TreeBox.Text) < 1 Then
                        set_style("WARNING: S-DIVA at least one binary trees" + vbCrLf, 1)
                        enable_sd = False
                    End If
                End If
            End If
            If CInt(BurninBox.Text) = 0 Then
                set_style("INFO: No tree is discarded. You could discard somes trees for S-DIVA and S-DEC" + vbCrLf, 3)
            End If
            set_style("CHECKING States:" + vbCrLf, 0)
            RangeStr = ""
            Dim get_null As Boolean = False
            For i As Integer = 1 To dtView.Count
                If dtView.Item(i - 1).Item(state_index).ToString = "" Then
                    get_null = True
                    set_style("WARNING: '" + dtView.Item(i - 1).Item(1).ToString + "' is NULL!" + vbCrLf, 1)
                    DataGridView1.Rows(i - 1).Selected = True
                End If
            Next
            For i As Integer = 1 To dtView.Count
                Dim Temp_d As String = dtView.Item(i - 1).Item(state_index).ToString.ToLower
                If Temp_d.Contains("p") Or Temp_d.Contains("q") Or Temp_d.Contains("r") Or Temp_d.Contains("s") Or Temp_d.Contains("t") Or Temp_d.Contains("u") Or Temp_d.Contains("v") Or Temp_d.Contains("w") Or Temp_d.Contains("x") Or Temp_d.Contains("y") Or Temp_d.Contains("z") Then
                    set_style("WARNING: More than 16 areas" + vbCrLf, 1)
                    enable_sd = False
                    Exit For
                End If
            Next
            If get_null Then
                enable_sd = False
                enable_dec = False
                enable_s_dec = False

                enable_bear = False
                enable_s_bear = False
                enable_bayestraits = False
                set_style("INFO: S-DIVA, S-DEC, DEC, BayesTraits and BioGeoBEARS do support NULL distribution!" + vbCrLf, 3)
            End If
            Dim letter_state As Boolean = False
            For i As Integer = 1 To dtView.Count
                For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                    If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                        If RangeStr.Contains(c) = False Then
                            RangeStr = RangeStr + c.ToString
                        End If
                    Else
                        enable_sd = False
                        enable_dec = False
                        enable_s_dec = False
                        enable_ba = False
                        enable_bb = False

                        enable_bear = False
                        enable_s_bear = False
                        enable_bayestraits = False
                        letter_state = True

                    End If
                Next
                If letter_state Then
                    set_style("INFO: S-DIVA, S-DEC, DEC, BBM, BayesTraits and BioGeoBEARS do support interval data!" + vbCrLf, 3)
                    set_style("INFO: Using Tools->Convert States to change the states to nominal data!" + vbCrLf, 3)
                    Exit For
                End If
            Next
            If letter_state = False Then
                TreeInfo.AppendText("Number of Areas: " + RangeStr.Length.ToString + vbCrLf)
                If RangeStr.Length < 2 Then
                    enable_sd = False
                    enable_dec = False
                    enable_s_dec = False
                    enable_ba = False
                    enable_bb = False

                    enable_s_bear = False
                    enable_bayestraits = False

                    enable_bear = False
                    set_style("ERROR: There should be 2 different areas at least!" + vbCrLf, 2)
                End If
                If RangeStr.Length >= 14 Then
                    set_style("INFO: If you have more than 26 areas, please use command line version of Bayarea!" + vbCrLf, 3)
                End If
                For i As Integer = 0 To RangeStr.Length - 1
                    If RangeStr.ToUpper.Contains(ChrW(65 + i)) = False Then
                        enable_sd = False
                        enable_dec = False
                        enable_s_dec = False
                        enable_ba = False
                        enable_bb = False

                        enable_s_bear = False
                        enable_bayestraits = False

                        enable_bear = False
                        set_style("ERROR: State should be Continuous letters! Missing area '" + ChrW(65 + i) + "'." + vbCrLf, 2)
                    End If
                Next
            End If

            If TargetOS = "x64" Then
                set_style("CHECKING R PACKAGES:" + vbCrLf, 0)
                If File.Exists(rscript) = False Then
                    enable_bear = False
                    enable_s_bear = False
                    set_style("ERROR: Could not find Rscript! BioGeoBEARS need R environment." + vbCrLf, 2)
                Else
                    set_style("INFO: gensa, snow, phylobase, biogeobears" + vbCrLf, 3)
                    Dim installed_packages As String = check_r_package({"gensa", "snow", "phylobase", "biogeobears"}, True)
                    set_style("INFO: " + installed_packages + vbCrLf, 3)
                    If installed_packages <> "Passed,Passed,Passed,Passed" Then
                        enable_bear = False

                        enable_s_bear = False
                        set_style("ERROR: Some packages were missing! Go to [Tools->Install 3rd Party->BioGeoBears] to install them." + vbCrLf, 2)
                    End If
                End If
            End If

        Else
            enable_sd = False
            enable_dec = False
            enable_s_dec = False
            enable_ba = False
            enable_bb = False
            enable_bt = False

            enable_bear = False

            enable_s_bear = False
            enable_bayestraits = False
            set_style(vbCrLf + "ERROR: No concensus tree!", 2)
        End If
        set_style(vbCrLf + "FINAL STATES:" + vbCrLf, 0)

        If enable_sd Then
            set_style("S-DIVA: Passed" + vbCrLf, 3)
        Else
            set_style("S-DIVA: Failed" + vbCrLf, 2)
        End If
        If enable_dec Then
            set_style("DEC: Passed" + vbCrLf, 3)
        Else
            set_style("DEC: Failed" + vbCrLf, 2)
        End If
        If enable_s_dec Then
            set_style("S-DEC: Passed" + vbCrLf, 3)
        Else
            set_style("S-DEC: Failed" + vbCrLf, 2)
        End If
        If enable_ba Then
            set_style("BayesArea: Passed" + vbCrLf, 3)
        Else
            set_style("BayesArea: Failed" + vbCrLf, 2)
        End If
        If enable_bb Then
            set_style("BBM: Passed" + vbCrLf, 3)
        Else
            set_style("BBM: Failed" + vbCrLf, 2)
        End If
        If enable_bear Then
            set_style("BioGeoBEARS: Passed" + vbCrLf, 3)
        Else
            set_style("BioGeoBEARS: Failed" + vbCrLf, 2)
        End If
        If enable_s_bear Then
            set_style("S-BioGeoBEARS: Passed" + vbCrLf, 3)
        Else
            set_style("S-BioGeoBEARS: Failed" + vbCrLf, 2)
        End If

        If enable_bayestraits Then
            set_style("Bayestraits: Passed" + vbCrLf, 3)
        Else
            set_style("Bayestraits: Failed" + vbCrLf, 2)
        End If

        'If enable_bt Then
        '    set_style("Bayestraits: Passed" + vbCrLf, 3)
        '    set_style("ChromEvol: Passed" + vbCrLf, 3)
        'Else
        '    set_style("Bayestraits: Failed" + vbCrLf, 2)
        '    set_style("ChromEvol: Failed" + vbCrLf, 2)
        'End If
        Write_Tree_Info = False

        set_style("S = Statistical" + vbCrLf, 3)
        If (enable_sd And enable_dec And enable_s_dec And enable_ba And enable_bb And enable_bt And enable_bear And enable_s_bear) = False Then
            If (enable_sd Or enable_dec Or enable_dec Or enable_bear Or enable_s_bear Or enable_ba Or enable_bb Or enable_bt) Then
                set_style(vbCrLf + "INFO: You could ignore the 'Failed' methods and use the 'Passed' methods." + vbCrLf, 3)
            Else
                set_style("No method passed, please check the error and warning messages!" + vbCrLf, 2)
            End If
        End If

    End Sub

    Private Sub TestToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        MsgBox(divapro("proc temp" + path_char + "divaproc.txt;" + vbCrLf, diva_gen))
    End Sub

    Private Sub Test2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        MsgBox(divapro("proc temp" + path_char + "divaproc1.txt;" + vbCrLf, diva_gen))
    End Sub

    Private Sub TraitsViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TraitsViewToolStripMenuItem.Click
        TraitsView.Show()
    End Sub

    Private Sub BatchToolToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BatchToolToolStripMenuItem.Click
        Dim b_t As New Batch_Tools
        b_t.Show()
    End Sub

    Private Sub ModelTestingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Public Sub runBGB()
        Try
            BGB_gen = 0
            Process_ID = 9
            current_dir = Directory.GetCurrentDirectory
            Directory.SetCurrentDirectory(root_path + "temp\")
            Dim startInfo As New ProcessStartInfo
            startInfo.FileName = "LOAD_BGB.bat"
            startInfo.WorkingDirectory = root_path + "temp"
            startInfo.UseShellExecute = False
            startInfo.CreateNoWindow = BGB_Config.CheckBox1.Checked
            startInfo.RedirectStandardOutput = BGB_Config.CheckBox1.Checked
            startInfo.RedirectStandardInput = BGB_Config.CheckBox1.Checked
            startInfo.RedirectStandardError = BGB_Config.CheckBox1.Checked
            startInfo.WindowStyle = ProcessWindowStyle.Minimized
            Process.Start(startInfo)
            Directory.SetCurrentDirectory(current_dir)
        Catch ex As Exception
            MsgBox("Something wrong! Check logs in temp folder for more information.")
        End Try

    End Sub

    Private Sub BGB_Timer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BGB_Timer.Tick
        If BGB_con_made Then
            BGB_con_made = False
            Me.Invoke(RT1, New Object() {vbCrLf + "*******************************************" + vbCrLf})
            Me.Invoke(RT1, New Object() {"*Using BioGeoBEARS Packages*" + vbCrLf})
            Me.Invoke(RT1, New Object() {"*******************************************" + vbCrLf})
            Me.Invoke(RT1, New Object() {"Process begin at " + Date.Now.ToString + vbCrLf})
            Select Case BGB_mode
                Case 0, 2, 3
                    Dim lb As New Thread(AddressOf runBGB)
                    lb.CurrentCulture = ci
                    lb.Start()
                Case 1
                    CheckForIllegalCrossThreadCalls = False
                    Dim lb As New Thread(AddressOf make_file_BGB)
                    lb.CurrentCulture = ci
                    lb.Start()
            End Select

            Disable_Windows()
            BGB_Timer.Enabled = False
        End If

    End Sub
    Public Sub read_geiger(ByVal res_file As String, ByVal tree_file As String, ByVal tree_line As String, ByVal model_name As String, ByVal append As Boolean)
        If File.Exists(res_file) Then
            If append = False Then
                rasp_result = "Geiger result file of " + state_header + vbCrLf
                rasp_result += "[TAXON]" + vbCrLf
                For i As Integer = 1 To taxon_num
                    rasp_result += i.ToString + "	" + dtView.Item(i - 1).Item(1).ToString + "	" + DataGridView1.Rows(i - 1).Cells(state_index + 1).Value + vbCrLf
                Next
            End If
            If tree_line = "" Then
                Dim sr_tree As New StreamReader(tree_file)
                tree_line = sr_tree.ReadLine
                sr_tree.Close()
            End If

            For i As Integer = 1 To taxon_num
                tree_line = tree_line.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ",", "($%*" + i.ToString + "$%*,")
                tree_line = tree_line.Replace("," + dtView.Item(i - 1).Item(1).ToString + ")", ",$%*" + i.ToString + "$%*)")
                tree_line = tree_line.Replace("," + dtView.Item(i - 1).Item(1).ToString + ",", ",$%*" + i.ToString + "$%*,")
                tree_line = tree_line.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ":", "($%*" + i.ToString + "$%*:")
                tree_line = tree_line.Replace("," + dtView.Item(i - 1).Item(1).ToString + ":", ",$%*" + i.ToString + "$%*:")
                tree_line = tree_line.Replace("," + dtView.Item(i - 1).Item(1).ToString + ":", ",$%*" + i.ToString + "$%*:")
            Next
            If append = False Then
                tree_line = tree_line.Replace("$%*", "")
                rasp_result += "[TREE]" + vbCrLf + "Tree=" + tree_line + vbCrLf
                rasp_result += "[RESULT]" + vbCrLf
            End If
            Dim sr As New StreamReader(res_file)

            Dim temp_result() As String
            ReDim temp_result(taxon_num - 1)
            For i As Integer = 1 To taxon_num - 1
                Dim line As String = sr.ReadLine
                temp_result(Left_to_right(i + taxon_num, tree_line) - taxon_num) = line
            Next
            sr.Close()
            Dim rang_num As Integer = RangeStr.Length
            Dim Tempchar() As Char = RangeStr.ToUpper
            Array.Sort(Tempchar)
            ReDim node_value(taxon_num - 1)
            rasp_result += model_name + " results:" + vbCrLf

            For i As Integer = 1 To taxon_num - 1
                rasp_result += "node " + (taxon_num + i).ToString + ":"

                Dim tmp_area_list() As String
                Dim result_value() As Single
                ReDim tmp_area_list(rang_num - 1)
                ReDim result_value(rang_num - 1)

                For k As Integer = 0 To rang_num - 1
                    result_value(k) = CSng(temp_result(i).Split(" ")(k))
                    tmp_area_list(k) = Tempchar(k)
                Next

                Array.Sort(result_value, tmp_area_list, New scomparer)
                For j As Integer = 0 To rang_num - 1
                    rasp_result += " " + tmp_area_list(j) + " " + (result_value(j) * 100).ToString("F6")
                Next
                rasp_result += vbCrLf
            Next
            File.Delete(res_file)
        Else
            MsgBox("Could not get Geiger result file! Check you data and setting please!")
        End If


    End Sub
    Public Sub read_BGB(ByVal res_file As String, ByVal tree_file As String, ByVal model_name As String, ByVal append As Boolean)
        If File.Exists(res_file) Then
            If append = False Then
                rasp_result = "BioGeoBEARS result file of " + state_header + vbCrLf
                rasp_result += "[TAXON]" + vbCrLf
                For i As Integer = 1 To taxon_num
                    rasp_result += i.ToString + "	" + dtView.Item(i - 1).Item(1).ToString + "	" + DataGridView1.Rows(i - 1).Cells(state_index + 1).Value + vbCrLf
                Next
            End If
            Dim treeline As String
            Dim sr_tree As New StreamReader(tree_file)
            treeline = sr_tree.ReadLine
            sr_tree.Close()
            For i As Integer = 1 To taxon_num
                treeline = treeline.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ",", "($%*" + i.ToString + "$%*,")
                treeline = treeline.Replace("," + dtView.Item(i - 1).Item(1).ToString + ")", ",$%*" + i.ToString + "$%*)")
                treeline = treeline.Replace("," + dtView.Item(i - 1).Item(1).ToString + ",", ",$%*" + i.ToString + "$%*,")
                treeline = treeline.Replace("(" + dtView.Item(i - 1).Item(1).ToString + ":", "($%*" + i.ToString + "$%*:")
                treeline = treeline.Replace("," + dtView.Item(i - 1).Item(1).ToString + ":", ",$%*" + i.ToString + "$%*:")
                treeline = treeline.Replace("," + dtView.Item(i - 1).Item(1).ToString + ":", ",$%*" + i.ToString + "$%*:")
            Next
            If append = False Then
                treeline = treeline.Replace("$%*", "")
                rasp_result += "[TREE]" + vbCrLf + "Tree=" + treeline + vbCrLf
                rasp_result += "[RESULT]" + vbCrLf
            End If
            Dim sr As New StreamReader(res_file)
            For i As Integer = 1 To taxon_num
                sr.ReadLine()
            Next
            Dim temp_result() As String
            ReDim temp_result(taxon_num - 1)
            For i As Integer = 1 To taxon_num - 1
                Dim line As String = sr.ReadLine
                temp_result(Left_to_right(i + taxon_num, treeline) - taxon_num) = line
            Next
            sr.Close()
            Dim rang_num As Integer = RangeStr.Length
            Dim Tempchar() As Char = RangeStr.ToUpper
            Dim area_list(0) As String
            area_list(0) = "/"

            Dim read_list As New StreamReader(root_path + "temp\state_list.txt")
            Dim temp_area As String = read_list.ReadLine
            temp_area = read_list.ReadLine
            Do
                If temp_area <> "" Then
                    ReDim Preserve area_list(UBound(area_list) + 1)
                    area_list(UBound(area_list)) = temp_area
                End If
                temp_area = read_list.ReadLine
            Loop Until temp_area Is Nothing
            read_list.Close()

            'Dim area_num As Integer = CInt(BGB_Config.NumericUpDown2.Value)
            'Array.Sort(Tempchar)
            'For j As Integer = 1 To area_num
            '    Dim n() As Integer
            '    ReDim n(j + 1)
            '    For x As Integer = 1 To j
            '        n(x) = x
            '    Next
            '    n(j + 1) = rang_num + 1
            '    Dim isend As Boolean = True
            '    Do
            '        Dim Tempstr As String = ""

            '        For x As Integer = 1 To j
            '            Tempstr = Tempstr + Tempchar(n(x) - 1)
            '        Next
            '        If BGB_Config.ListBox2.Items.IndexOf(Tempstr) < 0 Then
            '            ReDim Preserve area_list(UBound(area_list) + 1)
            '            area_list(UBound(area_list)) = Tempstr
            '        End If
            '        isend = pailie(n, j, j, rang_num)
            '    Loop Until isend = False
            'Next

            ReDim node_value(taxon_num - 1)
            rasp_result += model_name + " results:" + vbCrLf
            Dim result_value() As Single
            ReDim result_value(UBound(area_list))
            For i As Integer = 1 To taxon_num - 1
                rasp_result += "node " + (taxon_num + i).ToString + ":"
                Dim tmp_area_list() As String
                ReDim tmp_area_list(UBound(area_list))
                Dim temp_result_value() As String = temp_result(i).Split(" ")
                For j As Integer = 0 To UBound(area_list)
                    tmp_area_list(j) = area_list(j)
                    result_value(j) = CSng(temp_result_value(j))
                Next
                Array.Sort(result_value, tmp_area_list, New scomparer)
                For j As Integer = 0 To UBound(area_list)
                    rasp_result += " " + tmp_area_list(j) + " " + (result_value(j) * 100).ToString("F6")
                Next
                rasp_result += vbCrLf
            Next
            File.Delete(res_file)
        Else
            MsgBox("Could not get BioGeoBEARS result file! Check you data and setting please!")
        End If


    End Sub
    Public Function pailie(ByVal n() As Integer, ByVal postion As Integer, ByVal a_num As Integer, ByVal rang_num As Integer) As Boolean
        If n(postion) <= rang_num - (a_num - postion) And n(postion) <= n(postion + 1) - 2 Then
            n(postion) = n(postion) + 1
            Return True
        Else
            If postion > 1 Then
                If n(postion - 1) + 2 <= rang_num - (a_num - postion) Then
                    n(postion) = n(postion - 1) + 2
                End If
                For i As Integer = postion + 1 To a_num
                    If n(i - 1) + 1 <= rang_num - (a_num - (i - 1)) Then
                        n(i) = n(i - 1) + 1
                    End If
                Next
                For i As Integer = 1 To a_num
                    If n(i) > rang_num Then
                        Return False
                    End If
                Next
                Return pailie(n, postion - 1, a_num, rang_num)
            Else
                Return False
            End If
        End If
    End Function

    Private Sub SelectModelForTreesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub


    Private Sub InstallReinstallBioGeoBEARSToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub BioGeoBEARSToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BioGeoBEARSToolStripMenuItem1.Click
        If File.Exists(rscript) = False Then
            Dim Key1 As Microsoft.Win32.RegistryKey
            Key1 = My.Computer.Registry.LocalMachine
            Dim Key2 As Microsoft.Win32.RegistryKey
            Key2 = Key1.OpenSubKey("SOFTWARE\R-core\R", False)
            If (Key2.GetValue("InstallPath")) Is Nothing Then
                MsgBox("You should install R package before install BioGeoBEARS.")
                Dim opendialog As New OpenFileDialog
                opendialog.Filter = "Rscript.exe|Rscript.exe"
                opendialog.FileName = ""
                opendialog.Multiselect = False
                opendialog.DefaultExt = "Rscript.exe"
                opendialog.CheckFileExists = True
                opendialog.CheckPathExists = True
                Dim resultdialog As DialogResult = opendialog.ShowDialog()
                If resultdialog = DialogResult.OK Then
                    rscript = opendialog.FileName
                Else
                    Exit Sub
                End If
            Else
                rscript = Key2.GetValue("InstallPath") + "\bin\Rscript.exe"
            End If
        End If

        File.Copy(root_path + "Plug-ins\BGB\install.r", root_path + "temp\install.r", True)
        Dim sr As New StreamReader(root_path + "Plug-ins\BGB\install.r")
        Dim sw As New StreamWriter(root_path + "temp\install.r", False)

        sw.Write(sr.ReadToEnd.Replace("#lib_path#", lib_path))
        sr.Close()
        sw.Close()

        Dim wr5 As New StreamWriter(root_path + "temp\install.bat", False, System.Text.Encoding.Default)
        wr5.WriteLine("""" + rscript + """" + " install.r")
        wr5.Close()
        current_dir = Directory.GetCurrentDirectory
        Directory.SetCurrentDirectory(root_path + "temp\")
        Dim startInfo As New ProcessStartInfo
        startInfo.FileName = "install.bat"
        startInfo.WorkingDirectory = root_path + "temp"
        Process.Start(startInfo)
        Directory.SetCurrentDirectory(current_dir)
        Dim th1 As New Thread(AddressOf update_BGB)
        th1.Start()
    End Sub

    Private Sub RForWindowsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim res_dialog As DialogResult = MsgBox("Do you want to download and intsall R 3.3.3?", MsgBoxStyle.YesNo)
        If res_dialog = Windows.Forms.DialogResult.Yes Then
            Try
                My.Computer.Network.DownloadFile("https://cloud.r-project.org/bin/windows/base/old/3.3.3/R-3.3.3-win.exe", root_path + "temp/R-3.3.3-win.exe", "", "", True, 300000, True)
                If File.Exists(root_path + "temp/R-3.3.3-win.exe") Then
                    Shell(root_path + "temp/R-3.3.3-win.exe", AppWinStyle.NormalFocus)
                End If
            Catch ex As Exception
                If File.Exists(root_path + "temp/R-3.3.3-win.exe") Then
                    File.Delete(root_path + "temp/R-3.3.3-win.exe")
                End If
                Exit Sub
            End Try
        End If
    End Sub

    Private Sub FindRscriptexeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FindRscriptexeToolStripMenuItem.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Rscript.exe|Rscript.exe"
        opendialog.FileName = ""
        opendialog.Multiselect = False
        opendialog.DefaultExt = "Rscript.exe"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            rscript = opendialog.FileName
            Dim sw As New StreamWriter(root_path + "Plug-ins\R_path.txt")
            sw.WriteLine(rscript)
            sw.Close()
        Else
            Exit Sub
        End If
    End Sub

    Public Function check_r_package(ByVal name() As String, ByVal export_list As Boolean) As String
        Dim checking_times As Integer = 0
        Dim check_result() As String
        If File.Exists(root_path + "Plug-ins/BGB/packages.txt") = False Then
            export_list = True
        End If
        ReDim Preserve check_result(UBound(name))
        For i As Integer = 0 To UBound(name)
            check_result(i) = "Failed"
        Next
        If File.Exists(rscript) Then
            If export_list Then
                If File.Exists(root_path + "Plug-ins\BGB\packages.txt") Then
                    File.Delete(root_path + "Plug-ins\BGB\packages.txt")
                End If
                Dim sr As New StreamReader(root_path + "Plug-ins\BGB\installed.r")
                Dim sw As New StreamWriter(root_path + "temp\installed.r", False)
                sw.Write(sr.ReadToEnd.Replace("#lib_path#", lib_path))
                sw.Close()
                sr.Close()
                current_dir = Directory.GetCurrentDirectory
                Dim sw1 As New StreamWriter(root_path + "temp\check_install.bat")
                sw1.WriteLine("""" + rscript + """" + " installed.r")
                sw1.Close()
                Directory.SetCurrentDirectory(root_path + "temp\")
                Dim startInfo As New ProcessStartInfo
                startInfo.FileName = "check_install.bat"
                startInfo.WorkingDirectory = root_path + "temp"
                startInfo.UseShellExecute = False
                startInfo.CreateNoWindow = True
                startInfo.RedirectStandardOutput = True
                startInfo.RedirectStandardInput = True
                startInfo.RedirectStandardError = True
                startInfo.WindowStyle = ProcessWindowStyle.Minimized
                Process.Start(startInfo)
                Directory.SetCurrentDirectory(current_dir)
                Do
                    checking_times += 1
                    System.Threading.Thread.Sleep(100)
                Loop Until checking_times = 50 Or File.Exists(root_path + "Plug-ins\BGB\packages.txt")
            End If

            If File.Exists(root_path + "Plug-ins\BGB\packages.txt") Then
                Dim sr1 As New StreamReader(root_path + "Plug-ins\BGB\packages.txt")
                Dim line As String = sr1.ReadLine
                Do While (line Is Nothing) = False
                    For i As Integer = 0 To UBound(name)
                        If name(i).ToLower = line.ToLower.Split(";")(0) Then
                            check_result(i) = "Passed"
                        End If
                    Next
                    line = sr1.ReadLine
                Loop
                sr1.Close()
            End If
        End If
        Return Join(check_result, ",")
    End Function

    Private Sub BioGeoBEARSToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BioGeoBEARSToolStripMenuItem2.Click
        If TargetOS = "macos" Then
            BGB_Config.Label5.Visible = False
            BGB_Config.NumericUpDown1.Value = 1
            BGB_Config.NumericUpDown1.Visible = False
        End If
        DataGridView1.EndEdit()
        ProgressBar1.Maximum = 10000
        If CInt(TreeBox.Text) > 0 Then
            RangeStr = ""
            For i As Integer = 1 To dtView.Count
                If dtView.Item(i - 1).Item(state_index).ToString.Length = 0 Then
                    MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                    Exit Sub
                End If
                For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                    If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                        If RangeStr.Contains(c) = False Then
                            RangeStr = RangeStr + c.ToString
                        End If
                    Else
                        MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                        Exit Sub
                    End If
                Next
            Next
            For Each c As Char In RangeStr.ToUpper
                If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                    MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                    Exit Sub
                End If
            Next
            If RangeStr.Length = 1 Then
                MsgBox("There should be two different areas at least!")
                Exit Sub
            End If
            If final_tree.Replace(",", "").Length <> final_tree.Replace("(", "").Length Then
                MsgBox("Condensed tree contains " + (final_tree.Replace("(", "").Length - final_tree.Replace(",", "").Length).ToString + " polytomies!")
                Exit Sub
            End If
            If tree_show_with_value.Contains(":") = False Then
                MsgBox("You need a tree with branch length! Or use [Tools -> Add Branch Length] to generate branch length.")
                Exit Sub
            End If
            BGB_Config.NumericUpDown2.Maximum = RangeStr.Length
            If BGB_Config.NumericUpDown2.Value > RangeStr.Length Then
                BGB_Config.NumericUpDown2.Value = RangeStr.Length
            End If
            Dim sr As New StreamReader(root_path + "Plug-ins\R_path.txt")
            rscript = sr.ReadLine
            sr.Close()
            If File.Exists(rscript) = False Then
                Dim Key1 As Microsoft.Win32.RegistryKey
                Key1 = My.Computer.Registry.LocalMachine
                Dim Key2 As Microsoft.Win32.RegistryKey
                Key2 = Key1.OpenSubKey("SOFTWARE\R-core\R", False)
                If Key2 Is Nothing Then
                    MsgBox("You should install R and BioGeoBEARS package before use them.")
                    Dim opendialog As New OpenFileDialog
                    opendialog.Filter = "Rscript.exe|Rscript.exe"
                    opendialog.FileName = ""
                    opendialog.Multiselect = False
                    opendialog.DefaultExt = "Rscript.exe"
                    opendialog.CheckFileExists = True
                    opendialog.CheckPathExists = True
                    Dim resultdialog As DialogResult = opendialog.ShowDialog()
                    If resultdialog = DialogResult.OK Then
                        rscript = opendialog.FileName
                    Else
                        Exit Sub
                    End If
                Else
                    rscript = Key2.GetValue("InstallPath") + "\bin\Rscript.exe"
                End If
            End If

            If File.Exists(rscript) And TargetOS = "x64" Then
                Dim installed_packages As String = check_r_package({"gensa", "snow", "phylobase", "biogeobears"}, False)
                If installed_packages <> "Passed,Passed,Passed,Passed" Then
                    MsgBox("BioGeoBEARS packages were missing! " + vbCrLf + "Go to [Tools->Install 3rd Party->BioGeoBears] to install them.")
                    Exit Sub
                End If
            End If
            Dim sw As New StreamWriter(root_path + "Plug-ins\R_path.txt")
            sw.WriteLine(rscript)
            sw.Close()

            BGB_mode = 0
            BGB_con_made = False
            BGB_Timer.Enabled = True
            BGB_Config.Show()
            BGB_Config.ComboBox1.Visible = False
            BGB_Config.CheckBox2.Visible = True
            BGB_Config.Enabled = True
            BGB_Config.Label5.Text = "Cores"
            Dim r_cites As New StreamReader(root_path + "Plug-ins\CITES\BGB.txt")
            Me.Invoke(RT1, New Object() {r_cites.ReadToEnd})
            r_cites.Close()
        Else
            MsgBox("Need at least one binary tree!")
        End If
    End Sub

    Private Sub DIVALIKEInBiogenBEARSToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DIVALIKEInBiogenBEARSToolStripMenuItem.Click
        If TargetOS = "macos" Then
            BGB_Config.Label5.Visible = False
            BGB_Config.NumericUpDown1.Value = 1
            BGB_Config.NumericUpDown1.Visible = False
        End If
        DataGridView1.EndEdit()
        ProgressBar1.Maximum = 10000
        If CInt(TreeBox.Text) > 0 Then
            RangeStr = ""
            For i As Integer = 1 To dtView.Count
                If dtView.Item(i - 1).Item(state_index).ToString.Length = 0 Then
                    MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                    Exit Sub
                End If
                For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                    If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                        If RangeStr.Contains(c) = False Then
                            RangeStr = RangeStr + c.ToString
                        End If
                    Else
                        MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                        Exit Sub
                    End If
                Next
            Next
            For Each c As Char In RangeStr.ToUpper
                If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                    MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                    Exit Sub
                End If
            Next
            If RangeStr.Length = 1 Then
                MsgBox("There should be two different areas at least!")
                Exit Sub
            End If
            If final_tree.Replace(",", "").Length <> final_tree.Replace("(", "").Length Then
                MsgBox("Condensed tree contains " + (final_tree.Replace("(", "").Length - final_tree.Replace(",", "").Length).ToString + " polytomies!")
                Exit Sub
            End If
            If tree_show_with_value.Contains(":") = False Then
                MsgBox("You need a tree with branch length! Or use [Tools -> Add Branch Length] to generate branch length.")
                Exit Sub
            End If
            If BGB_Config.NumericUpDown2.Value > RangeStr.Length Then
                BGB_Config.NumericUpDown2.Value = RangeStr.Length
            End If
            Dim sr As New StreamReader(root_path + "Plug-ins\R_path.txt")
            rscript = sr.ReadLine
            sr.Close()
            If File.Exists(rscript) = False Then
                Dim Key1 As Microsoft.Win32.RegistryKey
                Key1 = My.Computer.Registry.LocalMachine
                Dim Key2 As Microsoft.Win32.RegistryKey
                Key2 = Key1.OpenSubKey("SOFTWARE\R-core\R", False)
                If Key2 Is Nothing Then
                    MsgBox("You should install R and BioGeoBEARS package before use them.")
                    Dim opendialog As New OpenFileDialog
                    opendialog.Filter = "Rscript.exe|Rscript.exe"
                    opendialog.FileName = ""
                    opendialog.Multiselect = False
                    opendialog.DefaultExt = "Rscript.exe"
                    opendialog.CheckFileExists = True
                    opendialog.CheckPathExists = True
                    Dim resultdialog As DialogResult = opendialog.ShowDialog()
                    If resultdialog = DialogResult.OK Then
                        rscript = opendialog.FileName
                    Else
                        Exit Sub
                    End If
                Else
                    rscript = Key2.GetValue("InstallPath") + "\bin\Rscript.exe"
                End If
            End If
            If File.Exists(rscript) And TargetOS = "x64" Then
                Dim installed_packages As String = check_r_package({"gensa", "snow", "phylobase", "biogeobears"}, False)
                If installed_packages <> "Passed,Passed,Passed,Passed" Then
                    MsgBox("BioGeoBEARS packages were missing! " + vbCrLf + "Go to [Tools->Install 3rd Party->BioGeoBears] to install them.")
                    Exit Sub
                End If
            End If
            Dim sw As New StreamWriter(root_path + "Plug-ins\R_path.txt")
            sw.WriteLine(rscript)
            sw.Close()

            BGB_mode = 2
            BGB_con_made = False
            BGB_Timer.Enabled = True
            BGB_Config.Show()
            BGB_Config.ComboBox1.Visible = True
            BGB_Config.CheckBox2.Visible = False
            BGB_Config.Enabled = True
            BGB_Config.Label5.Text = "Cores"
            Dim r_cites As New StreamReader(root_path + "Plug-ins\CITES\BGB.txt")
            Me.Invoke(RT1, New Object() {r_cites.ReadToEnd})
            r_cites.Close()
        Else
            MsgBox("Need at least one binary tree!")
        End If
    End Sub

    Private Sub StatisticalDIVALIKEToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatisticalDIVALIKEToolStripMenuItem.Click
        BGB_Config.CheckBox1.Visible = isDebug
        If TargetOS = "macos" Then
            BGB_Config.Label5.Visible = True
            BGB_Config.NumericUpDown1.Visible = True
        End If

        DataGridView1.EndEdit()
        ProgressBar1.Maximum = 10000
        If CInt(TreeBox.Text) > 1 Then
            RangeStr = ""
            For i As Integer = 1 To dtView.Count
                If dtView.Item(i - 1).Item(state_index).ToString.Length = 0 Then
                    MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                    Exit Sub
                End If
                For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                    If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                        If RangeStr.Contains(c) = False Then
                            RangeStr = RangeStr + c.ToString
                        End If
                    Else
                        MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                        Exit Sub
                    End If
                Next
            Next
            For Each c As Char In RangeStr.ToUpper
                If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                    MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                    Exit Sub
                End If
            Next
            If RangeStr.Length = 1 Then
                MsgBox("There should be two different areas at least!")
                Exit Sub
            End If
            'If final_tree.Replace(",", "").Length <> final_tree.Replace("(", "").Length Then
            '    MsgBox("Condensed tree contains " + (final_tree.Replace("(", "").Length - final_tree.Replace(",", "").Length).ToString + " polytomies!")
            '    Exit Sub
            'End If
            If tree_show_with_value.Contains(":") = False Then
                MsgBox("You need a tree with branch length! Or use [Tools -> Add Branch Length] to generate branch length.")
                Exit Sub
            End If
            If BGB_Config.NumericUpDown2.Value > RangeStr.Length Then
                BGB_Config.NumericUpDown2.Value = RangeStr.Length
            End If
            Dim sr As New StreamReader(root_path + "Plug-ins\R_path.txt")
            rscript = sr.ReadLine
            sr.Close()
            If File.Exists(rscript) = False Then
                Dim Key1 As Microsoft.Win32.RegistryKey
                Key1 = My.Computer.Registry.LocalMachine
                Dim Key2 As Microsoft.Win32.RegistryKey
                Key2 = Key1.OpenSubKey("SOFTWARE\R-core\R", False)
                If Key2 Is Nothing Then
                    MsgBox("You should install R and BioGeoBEARS package before use them.")
                    Dim opendialog As New OpenFileDialog
                    opendialog.Filter = "Rscript.exe|Rscript.exe"
                    opendialog.FileName = ""
                    opendialog.Multiselect = False
                    opendialog.DefaultExt = "Rscript.exe"
                    opendialog.CheckFileExists = True
                    opendialog.CheckPathExists = True
                    Dim resultdialog As DialogResult = opendialog.ShowDialog()
                    If resultdialog = DialogResult.OK Then
                        rscript = opendialog.FileName
                    Else
                        Exit Sub
                    End If
                Else
                    rscript = Key2.GetValue("InstallPath") + "\bin\Rscript.exe"
                End If
            End If
            Dim sw As New StreamWriter(root_path + "Plug-ins\R_path.txt")
            sw.WriteLine(rscript)
            sw.Close()
            If File.Exists(rscript) And TargetOS = "x64" Then
                Dim installed_packages As String = check_r_package({"gensa", "snow", "phylobase", "biogeobears"}, False)
                If installed_packages <> "Passed,Passed,Passed,Passed" Then
                    MsgBox("BioGeoBEARS packages were missing! " + vbCrLf + "Go to [Tools->Install 3rd Party->BioGeoBears] to install them.")
                    Exit Sub
                End If
            End If

            BGB_mode = 1
            BGB_con_made = False
            BGB_Timer.Enabled = True
            BGB_Config.Show()
            BGB_Config.ComboBox1.Visible = True
            BGB_Config.CheckBox2.Visible = False
            BGB_Config.Label5.Text = "Threads"
            BGB_Config.Enabled = True
            Dim r_cites As New StreamReader(root_path + "Plug-ins\CITES\BGB.txt")
            Me.Invoke(RT1, New Object() {r_cites.ReadToEnd})
            r_cites.Close()
        Else
            MsgBox("Need at least two binary trees!")
        End If
    End Sub

    Private Sub StatisticalDispersalVicarianceAnalysisSDIVAToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatisticalDispersalVicarianceAnalysisSDIVAToolStripMenuItem.Click
        DIVAForm.CheckBox13.Visible = isDebug

        If TargetOS = "macos" Then
            DIVAForm.CheckBox13.Visible = False
        End If
        DataGridView1.EndEdit()
        ProgressBar1.Maximum = 10000
        If IsNumeric(BurninBox.Text) And CInt(BurninBox.Text) <= CInt(TreeBox.Text) Then
            If CInt(TreeBox.Text) > 0 Then
                If dtView.Count > 512 Then
                    MsgBox("Your group of organisms should include no more than 512 taxa!")
                    Exit Sub
                End If
                RangeStr = ""
                For i As Integer = 1 To dtView.Count
                    For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                        If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                            If RangeStr.Contains(c) = False Then
                                RangeStr = RangeStr + c.ToString
                            End If
                        Else
                            MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters!")
                            Exit Sub
                        End If
                    Next
                Next
                For i As Integer = 1 To dtView.Count
                    If dtView.Item(i - 1).Item(state_index).ToString = "" Then
                        MsgBox("State should not be null!")
                        DataGridView1.Rows(i - 1).Selected = True
                        Exit Sub
                    End If
                    Dim Temp_d As String = dtView.Item(i - 1).Item(state_index).ToString.ToLower
                    If Temp_d.Contains("p") Or Temp_d.Contains("q") Or Temp_d.Contains("r") Or Temp_d.Contains("s") Or Temp_d.Contains("t") Or Temp_d.Contains("u") Or Temp_d.Contains("v") Or Temp_d.Contains("w") Or Temp_d.Contains("x") Or Temp_d.Contains("y") Or Temp_d.Contains("z") Then
                        MsgBox("State must be labeled using the letters A to O only!")
                        DataGridView1.Rows(i - 1).Selected = True
                        Exit Sub
                    End If
                Next
                If RangeStr.Length = 1 Then
                    MsgBox("There should be two different areas at least!")
                    Exit Sub
                End If
                For Each c As Char In RangeStr.ToUpper
                    If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                        MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                        Exit Sub
                    End If
                Next
                If final_tree.Replace(",", "").Length <> final_tree.Replace("(", "").Length Then
                    DIVAForm.CheckBox6.Checked = False
                    DIVAForm.CheckBox6.Enabled = False
                Else
                    DIVAForm.CheckBox6.Enabled = True
                End If
                If DIVAForm.NumericUpDown2.Value > RangeStr.Length Then
                    DIVAForm.NumericUpDown2.Value = RangeStr.Length
                End If
                DIVA_mode = 0
                RangeMade = False
                DIVAForm.Show()
                DIVA_Timer.Enabled = True
                Dim r_cites As New StreamReader(root_path + "Plug-ins\CITES\S-DIVA.txt")
                Me.Invoke(RT1, New Object() {r_cites.ReadToEnd})
                r_cites.Close()
            Else
                MsgBox("Need at least one binary tree!")
            End If

        Else
            MsgBox("Burn-in error!")
        End If
    End Sub

    Private Sub DispersalExtinctionCladogenesisDECToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DispersalExtinctionCladogenesisDECToolStripMenuItem.Click
        DataGridView1.EndEdit()
        ProgressBar1.Maximum = 10000
        Lagrange_Config.Label5.Visible = False
        Lagrange_Config.NumericUpDown1.Visible = False
        Lagrange_Config.CheckBox1.Visible = False

        RangeStr = ""
        For i As Integer = 1 To dtView.Count
            For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                    If RangeStr.Contains(c) = False Then
                        RangeStr = RangeStr + c.ToString
                    End If
                Else
                    MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                    Exit Sub
                End If
            Next
        Next
        For Each c As Char In RangeStr.ToUpper
            If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                Exit Sub
            End If
        Next
        If RangeStr.Length = 1 Then
            MsgBox("There should be two different areas at least!")
            Exit Sub
        End If
        If final_tree.Replace(",", "").Length <> final_tree.Replace("(", "").Length Then
            MsgBox("Condensed tree contains " + (final_tree.Replace("(", "").Length - final_tree.Replace(",", "").Length).ToString + " polytomies!")
            Exit Sub
        End If
        If tree_show_with_value.Contains(":") = False Then
            MsgBox("You need a tree with branch length! Or use [Tools -> Add Branch Length] to generate branch length.")
            Exit Sub
        End If
        If Lagrange_Config.NumericUpDown2.Value > RangeStr.Length Then
            Lagrange_Config.NumericUpDown2.Value = RangeStr.Length
        End If
        dec_mode = 0
        sdec_count = 1
        Lag_con_made = False
        Lagrange_Config.Show()
        Lag_Timer.Enabled = True
        Dim r_cites As New StreamReader(root_path + "Plug-ins\CITES\DEC.txt")
        Me.Invoke(RT1, New Object() {r_cites.ReadToEnd})
        r_cites.Close()
    End Sub

    Private Sub BayesianInferenceForDiscreteAreasBayAreaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BayesianInferenceForDiscreteAreasBayAreaToolStripMenuItem.Click

        If CInt(TreeBox_P.Text) > 0 Then
            DataGridView1.EndEdit()
            RangeStr = ""
            For i As Integer = 1 To dtView.Count
                For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                    If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                        If RangeStr.Contains(c) = False Then
                            RangeStr = RangeStr + c.ToString
                        End If
                    Else
                        MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z! " + Chr(13) + "If you have more than 26 areas, please use command line version of Bayarea!")
                        Exit Sub
                    End If
                Next
            Next
            For Each c As Char In RangeStr.ToUpper
                If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                    MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                    Exit Sub
                End If
            Next
            If FinalTreeBox.Text = "" Then
                MsgBox("Your should load a condense tree!")
                Exit Sub
            End If

            If RangeStr.Length = 0 Then
                MsgBox("There should be one areas at least!")
                Exit Sub
            End If
            If bayesIsrun Then
                MsgBox("Please wait analysis to complete!", MsgBoxStyle.Information)
                Exit Sub
            End If
            If tree_show_with_value.Contains(":") = False Then
                MsgBox("You need a tree with branch length! Or use [Tools -> Add Branch Length] to generate branch length.")
                Exit Sub
            End If
            BayAreaForm.Show()
            Dim r_cites As New StreamReader(root_path + "Plug-ins\CITES\BAYAREA.txt")
            Me.Invoke(RT1, New Object() {r_cites.ReadToEnd})
            r_cites.Close()
        Else
            MsgBox("Need at least one tree!")
        End If
    End Sub

    Private Sub CommandBuilderForBayesTraitsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        If CInt(TreeBox_P.Text) > 0 Then
            DataGridView1.EndEdit()
            If FinalTreeBox.Text = "" Then
                MsgBox("Your should load a condense tree!")
                Exit Sub
            End If

            If bayesIsrun Then
                MsgBox("Please wait analysis to complete!", MsgBoxStyle.Information)
                Exit Sub
            End If

            Dim og As String = ""
            For i As Integer = 1 To dtView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    og = i.ToString
                End If
            Next
            Config_Traits.Show()
        Else
            MsgBox("Need at least one tree!")
        End If
    End Sub

    Private Sub ChromosomeEvolutionChromEvolToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChromosomeEvolutionChromEvolToolStripMenuItem.Click
        If CInt(TreeBox_P.Text) > 0 Then
            DataGridView1.EndEdit()
            If FinalTreeBox.Text = "" Then
                MsgBox("Your should load a condense tree!")
                Exit Sub
            End If

            If bayesIsrun Then
                MsgBox("Please wait analysis to complete!", MsgBoxStyle.Information)
                Exit Sub
            End If

            Dim og As String = ""
            For i As Integer = 1 To dtView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    og = i.ToString
                End If
            Next
            get_tree_length(FinalTreeBox.Text.Substring(0, FinalTreeBox.Text.Length - 1))
            ChromForm.Show()
            Dim r_cites As New StreamReader(root_path + "Plug-ins\CITES\ChromEvol.txt")
            Me.Invoke(RT1, New Object() {r_cites.ReadToEnd})
            r_cites.Close()
        Else
            MsgBox("Need at least one tree!")
        End If
    End Sub

    Private Sub StatisticalDECSDECToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatisticalDECSDECToolStripMenuItem.Click

        Lagrange_Config.Label5.Visible = True
        Lagrange_Config.NumericUpDown1.Visible = True
        Lagrange_Config.CheckBox1.Visible = isDebug
        If TargetOS = "macos" Then
            Lagrange_Config.CheckBox1.Visible = False
        End If
        DataGridView1.EndEdit()
        ProgressBar1.Maximum = 10000
        If CInt(TreeBox.Text) > 1 Then
            RangeStr = ""
            For i As Integer = 1 To dtView.Count
                If dtView.Item(i - 1).Item(state_index).ToString.Length = 0 Then
                    MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                    Exit Sub
                End If
                For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                    If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                        If RangeStr.Contains(c) = False Then
                            RangeStr = RangeStr + c.ToString
                        End If
                    Else
                        MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                        Exit Sub
                    End If
                Next
            Next
            For Each c As Char In RangeStr.ToUpper
                If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                    MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                    Exit Sub
                End If
            Next
            If RangeStr.Length = 1 Then
                MsgBox("There should be two different areas at least!")
                Exit Sub
            End If
            'If final_tree.Replace(",", "").Length <> final_tree.Replace("(", "").Length Then
            '    MsgBox("Condensed tree contains " + (final_tree.Replace("(", "").Length - final_tree.Replace(",", "").Length).ToString + " polytomies!")
            '    Exit Sub
            'End If
            If tree_show_with_value.Contains(":") = False Then
                MsgBox("You need a tree with branch length! Or use [Tools -> Add Branch Length] to generate branch length.")
                Exit Sub
            End If
            If Lagrange_Config.NumericUpDown2.Value > RangeStr.Length Then
                Lagrange_Config.NumericUpDown2.Value = RangeStr.Length
            End If
            dec_mode = 1
            Lag_con_made = False
            Lagrange_Config.Show()
            Lag_Timer.Enabled = True
            Dim r_cites As New StreamReader(root_path + "Plug-ins\CITES\S-DEC.txt")
            Me.Invoke(RT1, New Object() {r_cites.ReadToEnd})
            r_cites.Close()
        Else
            MsgBox("Need at least two binary trees!")
        End If
    End Sub
    Public Sub update_BGB()
        Dim script_path As String = root_path + "Plug-ins\BGB\scripts\"
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/cladoRcpp.R", script_path + "cladoRcpp.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_add_fossils_randomly_v1.R", script_path + "BioGeoBEARS_add_fossils_randomly_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_basics_v1.R", script_path + "BioGeoBEARS_basics_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_calc_transition_matrices_v1.R", script_path + "BioGeoBEARS_calc_transition_matrices_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_classes_v1.R", script_path + "BioGeoBEARS_classes_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_detection_v1.R", script_path + "BioGeoBEARS_detection_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_DNA_cladogenesis_sim_v1.R", script_path + "BioGeoBEARS_DNA_cladogenesis_sim_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_extract_Qmat_COOmat_v1.R", script_path + "BioGeoBEARS_extract_Qmat_COOmat_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_generics_v1.R", script_path + "BioGeoBEARS_generics_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_models_v1.R", script_path + "BioGeoBEARS_models_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_on_multiple_trees_v1.R", script_path + "BioGeoBEARS_on_multiple_trees_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_plots_v1.R", script_path + "BioGeoBEARS_plots_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_readwrite_v1.R", script_path + "BioGeoBEARS_readwrite_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_simulate_v1.R", script_path + "BioGeoBEARS_simulate_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_SSEsim_makePlots_v1.R", script_path + "BioGeoBEARS_SSEsim_makePlots_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_SSEsim_v1.R", script_path + "BioGeoBEARS_SSEsim_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_stochastic_mapping_v1.R", script_path + "BioGeoBEARS_stochastic_mapping_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_stratified_v1.R", script_path + "BioGeoBEARS_stratified_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_univ_model_v1.R", script_path + "BioGeoBEARS_univ_model_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/calc_uppass_probs_v1.R", script_path + "calc_uppass_probs_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/calc_loglike_sp_v01.R", script_path + "calc_loglike_sp_v01.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/get_stratified_subbranch_top_downpass_likelihoods_v1.R", script_path + "get_stratified_subbranch_top_downpass_likelihoods_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/runBSM_v1.R", script_path + "runBSM_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/stochastic_map_given_inputs.R", script_path + "stochastic_map_given_inputs.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/summarize_BSM_tables_v1.R", script_path + "summarize_BSM_tables_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/BioGeoBEARS_traits_v1.R", script_path + "BioGeoBEARS_traits_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        My.Computer.Network.DownloadFile("http://phylo.wdfiles.com/local--files/biogeobears/_matrix_utils_v1.R", script_path + "BioGeoBEARS_matrix_utils_v1.R", "", "", True, 30000, True, FileIO.UICancelOption.DoNothing)
        Me.Invoke(RT1, New Object() {"Update scripts successfully!" + vbCrLf})
    End Sub

    Private Sub TreeTimeMultiplierToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TreeTimeMultiplierToolStripMenuItem.Click
        If FinalTreeBox.Text <> "" Then
            Dim Multiplier As String = InputBox("Multiply the branch length of final tree by:", "Tree Time Multiplier", "100.00")
            If Multiplier <> "" Then
                Try
                    Dim current_tree As String = FinalTreeBox.Text
                    current_tree = current_tree.Replace(";", "")
                    If current_tree.Contains(":") = False Then
                        MsgBox("Your final tree do not contain branch length!")
                        Me.Invoke(RT2_S, New Object() {""})
                        Process_Text = ""
                        Process_Int = 0
                        Process_ID = -1
                        Exit Sub
                    End If
                    Dim NumofTaxon As Integer = current_tree.Length - current_tree.Replace(",", "").Length + 1
                    Dim NumofNode As Integer = current_tree.Length - current_tree.Replace("(", "").Length
                    get_tree_length(current_tree)
                    Dim temp As String = ""
                    For i As Integer = 1 To Tree_Export_Char.Length - 1
                        If Tree_Export_Char(i).Contains(":") Then
                            If Tree_Export_Char(i - 1) <> ")" Then
                                temp += dtView.Item(CInt(Tree_Export_Char(i).Split(New Char() {":"c})(0)) - 1).Item(1).ToString + ":" + Max((Val(Tree_Export_Char(i).Split(New Char() {":"c})(1)) * Multiplier), 0.0001).ToString("F8")
                            Else
                                temp += Val(Tree_Export_Char(i).Split(New Char() {":"c})(0)).ToString + ":" + (Max(Val(Tree_Export_Char(i).Split(New Char() {":"c})(1)) * Multiplier, 0.0001)).ToString("F8")
                            End If
                        Else
                            temp += Tree_Export_Char(i)
                        End If
                    Next
                    temp += ";"
                    Dim sw As New StreamWriter(root_path + "temp" + path_char + "tree_multiply.tre")
                    sw.WriteLine(temp)
                    sw.Close()
                    tree_path = root_path + "temp" + path_char + "tree_multiply.tre"
                    Disable_Windows()
                    Process_Int = 0
                    Process_ID = 1
                    ProgressBar1.Maximum = 10000
                    mrbayes_tree = check_mrbayes()
                    Dim make_Tree As New Thread(AddressOf load_final_trees)
                    make_Tree.CurrentCulture = ci
                    make_Tree.Start()
                    MsgBox("The length from root to tip of your final tree is: " + (maxtime * Multiplier).ToString("F4") + " now." + vbCrLf + "Set minimal length to 0.0001" + vbCrLf + "Please reset rate matrix in DEC/S-DEC and BGB/S-BGB methods.")
                    Me.Invoke(RT1, New Object() {"Process end at " + Date.Now.ToString + vbCrLf})
                    BGB_Config.Button11.BackColor = Color.IndianRed
                    Lagrange_Config.Button11.BackColor = Color.IndianRed
                Catch ex As Exception

                End Try
            End If
        End If


    End Sub

    Private Sub AddTreeLengthToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddTreeLengthToolStripMenuItem.Click

        add_tree_length()

    End Sub

    Public Sub add_tree_length()
        If FinalTreeBox.Text <> "" Then
            Dim current_tree As String = FinalTreeBox.Text
            current_tree = current_tree.Replace(";", "")
            If current_tree.Contains(":") Then
                MsgBox("Your final tree already have branch length!")
                Me.Invoke(RT2_S, New Object() {""})
                Process_Text = ""
                Process_Int = 0
                Process_ID = -1
                Exit Sub
            End If
            Dim NumofTaxon As Integer = current_tree.Length - current_tree.Replace(",", "").Length + 1
            Dim NumofNode As Integer = current_tree.Length - current_tree.Replace("(", "").Length
            get_tree_length(current_tree)
            Dim temp As String = ""
            Dim level As Integer = 0
            Dim max_level As Integer = 0
            For i As Integer = 1 To Tree_Export_Char.Length - 1
                If Tree_Export_Char(i - 1) = "(" Then
                    level += 1
                End If
                If Tree_Export_Char(i - 1) = ")" Then
                    level -= 1
                End If
                If max_level < level Then
                    max_level = level
                End If
            Next
            level = 0
            For i As Integer = 1 To Tree_Export_Char.Length - 1
                If Tree_Export_Char(i) = "," Then
                    temp += Tree_Export_Char(i)
                    Continue For
                End If
                If Tree_Export_Char(i) = "(" Then
                    level += 1
                End If
                If Tree_Export_Char(i) = ")" Then
                    level -= 1
                End If
                If Tree_Export_Char(i) = ")" Then
                    Dim temp_arr() As String = {",", ")", ";"}
                    If i < Tree_Export_Char.Length - 1 Then
                        If Array.IndexOf(temp_arr, Tree_Export_Char(i + 1)) >= 0 Then
                            temp += Tree_Export_Char(i) + ":1"
                        Else
                            temp += Tree_Export_Char(i) + Tree_Export_Char(i + 1) + ":1"
                            i += 1
                            Continue For
                        End If
                    Else
                        temp += Tree_Export_Char(i)
                    End If
                ElseIf i < Tree_Export_Char.Length - 1 Then
                    If (Tree_Export_Char(i - 1) = "(" And Tree_Export_Char(i + 1) = ",") Or (Tree_Export_Char(i - 1) = "," And Tree_Export_Char(i + 1) = ")") Or (Tree_Export_Char(i - 1) = "," And Tree_Export_Char(i + 1) = ",") Then
                        temp += Tree_Export_Char(i) + ":" + (max_level - level + 1).ToString
                    Else
                        temp += Tree_Export_Char(i)
                    End If
                Else
                    temp += Tree_Export_Char(i)
                End If
            Next
            temp += ";"
            Dim sw As New StreamWriter(root_path + "temp" + path_char + "tree_multiply.tre")
            sw.WriteLine(temp)
            sw.Close()
            tree_path = root_path + "temp" + path_char + "tree_multiply.tre"
            Disable_Windows()
            Process_Int = 0
            Process_ID = 1
            ProgressBar1.Maximum = 10000
            mrbayes_tree = check_mrbayes()
            Dim make_Tree As New Thread(AddressOf load_final_trees)
            make_Tree.CurrentCulture = ci
            make_Tree.Start()
            Me.Invoke(RT1, New Object() {"Process end at " + Date.Now.ToString + vbCrLf})
            BGB_Config.Button11.BackColor = Color.IndianRed
            Lagrange_Config.Button11.BackColor = Color.IndianRed
        End If
    End Sub

    Private Sub TreeClusterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TreeClusterToolStripMenuItem.Click
        DataGridView1.EndEdit()
        If ClusterForm = False Then
            If CInt(TreeBox_P.Text) > 1 Then
                Dim name_w As New StreamWriter(root_path + "temp\taxon_name.txt")
                For i As Integer = 1 To dtView.Count
                    If dtView.Item(i - 1).Item(0).ToString <> "" And dtView.Item(i - 1).Item(1).ToString <> "" Then
                        name_w.WriteLine(dtView.Item(i - 1).Item(1).ToString)
                    End If
                Next
                name_w.Close()
                Dim new_form As New Tool_Cluster
                new_form.Show()
                ClusterForm = True
                Dim cites As New StreamReader(root_path + "Plug-ins\CITES\Cluster.txt")
                Me.Invoke(RT1, New Object() {cites.ReadToEnd})
                cites.Close()
            Else
                MsgBox("You should have 2 trees at least!")
            End If
        End If
    End Sub

    Private Sub StatesVsTreesToolStripMenuItem_Click(sender As Object, e As EventArgs)
        DataGridView1.EndEdit()
        If CInt(TreeBox_P.Text) >= 1 Then
            state_mode = 0
            For i As Integer = 1 To dtView.Count
                If IsNumeric(dtView.Item(i - 1).Item(state_index)) Then
                    state_mode = 1
                    Exit For
                End If
            Next
            Select Case state_mode
                Case 0
                    RangeStr = ""
                    For i As Integer = 1 To dtView.Count
                        For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                            If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                                If RangeStr.Contains(c) = False Then
                                    RangeStr = RangeStr + c.ToString
                                End If
                            Else
                                MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                                Exit Sub
                            End If
                        Next
                    Next
                    For Each c As Char In RangeStr.ToUpper
                        If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                            MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                            Exit Sub
                        End If
                    Next
                    If RangeStr.Length = 1 Then
                        MsgBox("There should be two different states at least!")
                        Exit Sub
                    End If
                Case 1

            End Select

            SvTForm.Show()
        Else
            MsgBox("You should have 1 tree at least!")
        End If

    End Sub

    Private Sub DataGridView1_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView1.ColumnHeaderMouseClick
        If e.ColumnIndex >= 3 Then
            set_column_style(e.ColumnIndex)
        End If

    End Sub
    Public Sub set_column_style(ByVal index As Integer)
        state_index = index - 1
        For j As Integer = 3 To DataGridView1.ColumnCount - 1
            For k As Integer = 0 To DataGridView1.RowCount - 1
                If j = index Then
                    DataGridView1.Rows(k).Cells(j).Style.BackColor = Color.FromArgb(153, 204, 51)
                Else
                    DataGridView1.Rows(k).Cells(j).Style.BackColor = Color.White
                End If
            Next
        Next
    End Sub
    Private Sub ComparisonToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ComparisonToolStripMenuItem.Click

    End Sub

    Private Sub ProcessToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ProcessToolStripMenuItem.Click

    End Sub


    Private Sub TreeVsStatesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TreeVsStatesToolStripMenuItem.Click
        If FinalTreeBox.Text <> "" Then
            TvSForm.Show()
            Dim cites As New StreamReader(root_path + "Plug-ins\CITES\States.txt")
            Me.Invoke(RT1, New Object() {cites.ReadToEnd})
            cites.Close()
        Else
            MsgBox("You should have a final tree!")
        End If

    End Sub

    Private Sub BayesianBinaryMCMCBBMToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles BayesianBinaryMCMCBBMToolStripMenuItem1.Click

        If CInt(TreeBox_P.Text) > 0 Then
            DataGridView1.EndEdit()
            RangeStr = ""
            For i As Integer = 1 To dtView.Count
                For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                    If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                        If RangeStr.Contains(c) = False Then
                            RangeStr = RangeStr + c.ToString
                        End If
                    Else
                        MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters!")
                        Exit Sub
                    End If
                Next
            Next
            For Each c As Char In RangeStr.ToUpper
                If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                    MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                    Exit Sub
                End If
            Next
            If FinalTreeBox.Text = "" Then
                MsgBox("Your should load a condense tree!")
                Exit Sub
            End If

            If RangeStr.Length = 0 Then
                MsgBox("There should be one areas at least!")
                Exit Sub
            End If


            If bayesIsrun Then
                MsgBox("Please wait analysis to complete!", MsgBoxStyle.Information)
                Exit Sub
            End If
            Dim og As String = ""
            For i As Integer = 1 To dtView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    og = i.ToString
                End If
            Next
            BayesForm.Show()
            Dim r_cites As New StreamReader(root_path + "Plug-ins\CITES\BBM.txt")
            Me.Invoke(RT1, New Object() {r_cites.ReadToEnd})
            r_cites.Close()
        Else
            MsgBox("Need at least one tree!")
        End If
    End Sub
    Private Sub ERSYMARDModelInAPEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ERSYMARDModelInAPEToolStripMenuItem.Click
        DataGridView1.EndEdit()
        ProgressBar1.Maximum = 10000

        RangeStr = ""
        For i As Integer = 1 To dtView.Count
            For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                    If RangeStr.Contains(c) = False Then
                        RangeStr = RangeStr + c.ToString
                    End If
                Else
                    MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                    Exit Sub
                End If
            Next
        Next
        For Each c As Char In RangeStr.ToUpper
            If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                Exit Sub
            End If
        Next
        If RangeStr.Length = 1 Then
            MsgBox("There should be two different areas at least!")
            Exit Sub
        End If
        If final_tree.Replace(",", "").Length <> final_tree.Replace("(", "").Length Then
            MsgBox("Condensed tree contains " + (final_tree.Replace("(", "").Length - final_tree.Replace(",", "").Length).ToString + " polytomies!")
            Exit Sub
        End If
        If tree_show_with_value.Contains(":") = False Then
            MsgBox("You need a tree with branch length! Or use [Tools -> Add Branch Length] to generate branch length.")
            Exit Sub
        End If

        Dim temp_mode As Integer = 0
        Dim isNum As Boolean = True
        Dim isSingle As Boolean = True
        For j As Integer = 1 To dtView.Count
            If IsNumeric(dtView.Item(j - 1).Item(state_index)) = False Then
                isNum = False
            End If
            If dtView.Item(j - 1).Item(state_index).ToString.Length > 1 Then
                isSingle = False
            End If
        Next
        If isNum Then
            temp_mode = 1
            MsgBox("This method do not support continuous states.")
            Exit Sub
        ElseIf isSingle Then
            temp_mode = 2
        Else
            temp_mode = 0
            MsgBox("This method do not support species with multistate.")
            Exit Sub
        End If

        If File.Exists(root_path + "temp\ACE.state") Then
            File.Delete(root_path + "temp\ACE.state")
        End If
        If File.Exists(root_path + "temp\ACE.end") Then
            File.Delete(root_path + "temp\ACE.end")
        End If
        Disable_Windows()
        Dim th1 As New Threading.Thread(AddressOf ACE)
        th1.Start(temp_mode)

        Dim cites As New StreamReader(root_path + "Plug-ins\CITES\APE.txt")
        Me.Invoke(RT1, New Object() {cites.ReadToEnd})
        cites.Close()


    End Sub
    Public Sub ACE(ByVal temp_mode As Integer)
        Dim sr_header As New StreamReader(root_path + "Plug-ins\ACE\header.R")
        Dim ace_header As String = sr_header.ReadToEnd.Replace("#lib_path#", lib_path)
        sr_header.Close()
        Dim sw_header As New StreamWriter(root_path + "temp\run_ace.r", False, System.Text.Encoding.Default)
        sw_header.Write(ace_header)
        sw_header.Close()

        claculate_ace(temp_mode, state_index, FinalTreeBox.Text, 1)

        Dim sr_footer As New StreamReader(root_path + "Plug-ins\ACE\footer.R")
        Dim ace_footer As String = sr_footer.ReadToEnd
        sr_footer.Close()
        Dim sw_footer As New StreamWriter(root_path + "temp\run_ace.r", True, System.Text.Encoding.Default)
        sw_footer.Write(ace_footer)
        sw_footer.Close()

        Dim sr_run As New StreamWriter(root_path + "temp\run_ace.bat", False, System.Text.Encoding.Default)
        sr_run.WriteLine("""" + rscript + """" + " run_ace.r")
        sr_run.Close()

        current_dir = Directory.GetCurrentDirectory
        Directory.SetCurrentDirectory(root_path + "temp\")
        Dim startInfo As New ProcessStartInfo
        startInfo.FileName = "run_ace.bat"
        startInfo.WorkingDirectory = root_path + "temp"
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = False 'HideCMDWindowToolStripMenuItem.Checked
        'startInfo.RedirectStandardOutput = HideCMDWindowToolStripMenuItem.Checked
        'startInfo.RedirectStandardInput = HideCMDWindowToolStripMenuItem.Checked
        'startInfo.RedirectStandardError = HideCMDWindowToolStripMenuItem.Checked
        Process.Start(startInfo)
        Directory.SetCurrentDirectory(current_dir)
        Process_Int = 0
        Process_ID = 13
    End Sub
    Public Sub claculate_ace(ByVal temp_mode As Integer, ByVal current_index As Integer, ByVal input_tree As String, ByVal progress_id As Integer)

        Dim sr_body As New StreamReader(root_path + "Plug-ins\ACE\body.R")
        Dim ace_body As String = sr_body.ReadToEnd
        sr_body.Close()
        Dim sw_ace As New StreamWriter(root_path + "temp\run_ace.r", True, System.Text.Encoding.Default)

        Dim trait_value() As String
        ReDim trait_value(dtView.Count - 1)
        Dim trait_name As String = "c('"
        trait_name &= join_array(make_seq(dtView.Count, 1), "','") & "')"

        Dim trait_value_text As String = ""
        Select Case temp_mode
            Case 1
                For i As Integer = 1 To dtView.Count
                    trait_value(i - 1) = CSng(dtView.Item(i - 1).Item(current_index))
                Next
                trait_value_text = "c(" + join_array(trait_value, ",") & ")"
            Case 2
                For i As Integer = 1 To dtView.Count
                    If dtView.Item(i - 1).Item(current_index).ToString <> "" And dtView.Item(i - 1).Item(current_index).ToString <> "?" Then
                        trait_value(i - 1) = dtView.Item(i - 1).Item(current_index).ToString
                    Else
                        trait_value(i - 1) = "NA"
                    End If
                Next
                trait_value_text = "c('" + join_array(trait_value, "','") & "')".Replace("'NA'", "NA")
            Case Else
                For i As Integer = 1 To dtView.Count
                    trait_value(i - 1) = dtView.Item(i - 1).Item(current_index)
                Next
                trait_value_text = "c('" + join_array(trait_value, "','") & "')"
        End Select

        ace_body = ace_body.Replace("#trait_value#", trait_value_text)
        ace_body = ace_body.Replace("#trait_name#", trait_name).Replace("#tree_value#", input_tree)
        ace_body = ace_body.Replace("#trait_id#", current_index - 1)
        ace_body = ace_body.Replace("#progress#", progress_id)
        ace_body = ace_body.Replace("#trait_mode#", temp_mode)
        ace_body = ace_body.Replace("#trait_title#", MainWindow.DataGridView1.Columns(current_index + 1).HeaderText)

        sw_ace.Write(ace_body)
        sw_ace.Close()
    End Sub

    Private Sub StatisticalERSYMARDInAPEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StatisticalERSYMARDInAPEToolStripMenuItem.Click
        DataGridView1.EndEdit()
        ProgressBar1.Maximum = 10000

        If CInt(TreeBox.Text) > 1 Then
            RangeStr = ""
            For i As Integer = 1 To dtView.Count
                For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                    If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                        If RangeStr.Contains(c) = False Then
                            RangeStr = RangeStr + c.ToString
                        End If
                    Else
                        MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters from A to Z!")
                        Exit Sub
                    End If
                Next
            Next
            For Each c As Char In RangeStr.ToUpper
                If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                    MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                    Exit Sub
                End If
            Next
            If RangeStr.Length = 1 Then
                MsgBox("There should be two different areas at least!")
                Exit Sub
            End If
            If final_tree.Replace(",", "").Length <> final_tree.Replace("(", "").Length Then
                MsgBox("Condensed tree contains " + (final_tree.Replace("(", "").Length - final_tree.Replace(",", "").Length).ToString + " polytomies!")
                Exit Sub
            End If
            If tree_show_with_value.Contains(":") = False Then
                MsgBox("You need a tree with branch length! Or use [Tools -> Add Branch Length] to generate branch length.")
                Exit Sub
            End If

            Dim temp_mode As Integer = 0
            Dim isNum As Boolean = True
            Dim isSingle As Boolean = True
            For j As Integer = 1 To dtView.Count
                If IsNumeric(dtView.Item(j - 1).Item(state_index)) = False Then
                    isNum = False
                End If
                If dtView.Item(j - 1).Item(state_index).ToString.Length > 1 Then
                    isSingle = False
                End If
            Next
            If isNum Then
                temp_mode = 1
                MsgBox("This method do not support continuous states.")
                Exit Sub
            ElseIf isSingle Then
                temp_mode = 2
            Else
                temp_mode = 0
                MsgBox("This method do not support species with multistate.")
                Exit Sub
            End If

            If File.Exists(root_path + "temp\ACE.state") Then
                File.Delete(root_path + "temp\ACE.state")
            End If
            If File.Exists(root_path + "temp\ACE.end") Then
                File.Delete(root_path + "temp\ACE.end")
            End If
            Disable_Windows()
            SACE_count = 0
            Dim th1 As New Threading.Thread(AddressOf make_file_ape)
            th1.Start(temp_mode)

            Dim cites As New StreamReader(root_path + "Plug-ins\CITES\APE.txt")
            Me.Invoke(RT1, New Object() {cites.ReadToEnd})
            cites.Close()
        Else
            MsgBox("Need at least two binary tree!")
        End If
    End Sub
    Public Sub make_file_ape(ByVal temp_mode As Integer)
        Dim current_tree As String
        Dim rt As New StreamReader(root_path + "temp" + path_char + "clean_num.trees")
        Dim burn_in As Integer = CInt(BurninBox.Text)
        Do While burn_in > 0
            rt.ReadLine()
            burn_in = burn_in - 1
        Loop
        Dim seed As Integer = DateTime.Now.Millisecond
        If Global_seed <> "20180127" Then
            seed = Global_seed
        End If
        Dim rand As New System.Random(seed)
        Dim is_random As Boolean = False
        Dim random_num As Integer = 0
        Dim random_array(0) As Integer
        Try
            Process_Int = 0
            Process_ID = 1
            Dim tree_num As Integer = 1
            Me.Invoke(RT2_S, New Object() {"Making command ..."})



            Dim sr_header As New StreamReader(root_path + "Plug-ins\ACE\header.R")
            Dim ace_header As String = sr_header.ReadToEnd.Replace("#lib_path#", lib_path)
            sr_header.Close()
            Dim sw_header As New StreamWriter(root_path + "temp\run_ace.r", False, System.Text.Encoding.Default)
            sw_header.Write(ace_header)
            sw_header.Close()



            Dim Analyse_trees As New StreamWriter(root_path + "temp" + path_char + "random_trees.tre", False)
            For t As Integer = CInt(BurninBox.Text) + 1 To CInt(TreeBox.Text)
                If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
                    Dim random_w As New StreamWriter(root_path + "temp" + path_char + "random_num.txt", False)
                    ReDim random_array(CInt(RandomTextBox.Text))
                    random_w.WriteLine(RandomTextBox.Text)
                    is_random = True
r2:                 If random_num < CInt(RandomTextBox.Text) Then
                        random_num = random_num + 1
                        t = rand.Next(CInt(BurninBox.Text) + 1, CInt(TreeBox.Text))
                        random_array(random_num) = t
                        random_w.WriteLine(t.ToString)
                        GoTo n_r1
                    End If
                    random_w.Close()
                    Exit For
                End If
n_r1:           If CheckBox3.Checked And CInt(RandomTextBox.Text) > 0 Then
                    Dim random_rt As New StreamReader(root_path + "temp" + path_char + "clean_num.trees")
                    For i As Integer = 1 To t - 1
                        random_rt.ReadLine()
                    Next
                    current_tree = random_rt.ReadLine()
                    random_rt.Close()
                Else
                    current_tree = rt.ReadLine()
                End If
                Analyse_trees.WriteLine(current_tree)
                If File.Exists(root_path + "temp" + path_char + t.ToString + ".txt") Then
                    File.Delete(root_path + "temp" + path_char + t.ToString + ".txt")
                End If

                Dim savepath As String = (root_path + "temp" + path_char + "temp_ACE").Replace("\", "/")
                current_tree = current_tree.Replace(";", "")
                If current_tree.Contains(":") = False Then
                    Me.Invoke(RT1, New Object() {"Warning: Your trees dataset do not contain branch length!" + vbCrLf})
                    Me.Invoke(RT1, New Object() {"Try to add branch length to trees with [Tools -> Add Branch Length]." + vbCrLf})
                    Me.Invoke(RT2_S, New Object() {""})
                    current_tree = current_tree.Replace(")", "):0.0001")
                End If
                '把没有支长的分支加上0.01，从而兼容ACE
                current_tree = current_tree.Replace("):", "#$:#")
                current_tree = current_tree.Replace(")", "):0.0001")
                current_tree = current_tree.Replace("#$:#", "):")

                claculate_ace(temp_mode, state_index, current_tree + ";", tree_num.ToString)
                SACE_count += 1
                If is_random Then
                    PV_SUM = CInt(RandomTextBox.Text)
                    Process_Int = CInt(random_num / PV_SUM * 10000)
                    tree_num += 1
                    GoTo r2
                End If
                PV_SUM = (CInt(TreeBox.Text) - CInt(BurninBox.Text))
                Process_Int = CInt((t - CInt(BurninBox.Text)) / PV_SUM * 10000)
                tree_num += 1
            Next
            Analyse_trees.Close()



            Dim sr_footer As New StreamReader(root_path + "Plug-ins\ACE\footer.R")
            Dim ace_footer As String = sr_footer.ReadToEnd
            sr_footer.Close()
            Dim sw_footer As New StreamWriter(root_path + "temp\run_ace.r", True, System.Text.Encoding.Default)
            sw_footer.Write(ace_footer)
            sw_footer.Close()

            Dim sr_run As New StreamWriter(root_path + "temp\run_ace.bat", False, System.Text.Encoding.Default)
            sr_run.WriteLine("""" + rscript + """" + " run_ace.r")
            sr_run.Close()

            current_dir = Directory.GetCurrentDirectory
            Directory.SetCurrentDirectory(root_path + "temp\")
            Dim startInfo As New ProcessStartInfo
            startInfo.FileName = "run_ace.bat"
            startInfo.WorkingDirectory = root_path + "temp"
            startInfo.UseShellExecute = False
            startInfo.CreateNoWindow = False 'HideCMDWindowToolStripMenuItem.Checked
            'startInfo.RedirectStandardOutput = HideCMDWindowToolStripMenuItem.Checked
            'startInfo.RedirectStandardInput = HideCMDWindowToolStripMenuItem.Checked
            'startInfo.RedirectStandardError = HideCMDWindowToolStripMenuItem.Checked
            Process.Start(startInfo)
            Directory.SetCurrentDirectory(current_dir)
            Process_Int = 0
            Process_ID = 14

            Me.Invoke(RT2_S, New Object() {"APE package is loading..."})
            CheckForIllegalCrossThreadCalls = True
        Catch ex As Exception
            MsgBox(ex.ToString)
            MsgBox("Cannot process the trees!")
            Exit Sub
        End Try
        rt.Close()
    End Sub

    Private Sub MultiStateReconstructionInBayesTraitsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MultiStateReconstructionInBayesTraitsToolStripMenuItem.Click
        If CInt(TreeBox_P.Text) > 0 Then
            DataGridView1.EndEdit()
            If FinalTreeBox.Text = "" Then
                MsgBox("Your should load a condense tree!")
                Exit Sub
            End If

            If bayesIsrun Then
                MsgBox("Please wait analysis to complete!", MsgBoxStyle.Information)
                Exit Sub
            End If
            RangeStr = ""
            For i As Integer = 1 To dtView.Count
                For Each c As Char In dtView.Item(i - 1).Item(state_index).ToString.ToUpper
                    If Asc(c) >= Asc("A") And Asc(c) <= Asc("Z") Then
                        If RangeStr.Contains(c) = False Then
                            RangeStr = RangeStr + c.ToString
                        End If
                    Else
                        MsgBox("State of Taxon " + dtView.Item(i - 1).Item(0).ToString + " should be letters!")
                        Exit Sub
                    End If
                Next
            Next
            For Each c As Char In RangeStr.ToUpper
                If AscW(c) - AscW("A") + 1 > RangeStr.Length Then
                    MsgBox("State should be Continuous letters! Please alter area '" + c + "'.")
                    Exit Sub
                End If
            Next
            Dim og As String = ""
            For i As Integer = 1 To dtView.Count
                If DataGridView1.Rows(i - 1).Cells(0).FormattedValue.ToString = "True" Then
                    og = i.ToString
                End If
            Next

            Dim cites As New StreamReader(root_path + "Plug-ins\CITES\BayesTraits.txt")
            Me.Invoke(RT1, New Object() {cites.ReadToEnd})
            cites.Close()

            Config_Traits.Show()
        Else
            MsgBox("Need at least one tree!")
        End If
    End Sub


    Private Sub SetSeedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetSeedToolStripMenuItem.Click
        Try
            Dim seed As Integer = DateTime.Now.Millisecond
            Global_seed = InputBox("Please input an integer", "Seed", seed)
        Catch ex As Exception
            MsgBox("Wrong seed!")
        End Try

    End Sub


    Private Sub ConvertStatesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConvertStatesToolStripMenuItem.Click
        For i As Integer = 1 To dtView.Count
            If IsNumeric(dtView.Item(i - 1).Item(state_index)) = False And dtView.Item(i - 1).Item(state_index).ToString <> "" Then
                MsgBox("Could not convert states!")
                Exit Sub
            End If
        Next

        Dim form1 As New Tool_Convert
        form1.Show()
    End Sub

    Private Sub AddStatesToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles AddStatesToolStripMenuItem1.Click
        Dim temp_name As String = InputBox("Please input the name of new state", "new state", "State")
        If temp_name <> "" Then
            Try
                Taxon_Dataset.Tables("Taxon Table").Columns.Add(temp_name)
                For i As Integer = 2 To DataGridView1.Columns.Count - 1
                    DataGridView1.Columns(i).SortMode = DataGridViewColumnSortMode.NotSortable
                Next
                dtView = MainWindow.Taxon_Dataset.Tables("Taxon Table").DefaultView
                MainWindow.DataGridView1.Sort(MainWindow.DataGridView1.Columns(1), System.ComponentModel.ListSortDirection.Ascending)
            Catch ex As Exception
                MsgBox("The name of column is repeated! Please try again!")
            End Try
        End If
    End Sub

    Private Sub DeleteStatesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteStatesToolStripMenuItem.Click
        If state_index > 0 Then
            Taxon_Dataset.Tables("Taxon Table").Columns.RemoveAt(state_index)
            dtView = MainWindow.Taxon_Dataset.Tables("Taxon Table").DefaultView
            MainWindow.DataGridView1.Sort(MainWindow.DataGridView1.Columns(1), System.ComponentModel.ListSortDirection.Ascending)
            state_index = -1
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub 中文ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 中文ToolStripMenuItem.Click
        If language = "EN" Then
            to_ch()
        Else
            to_en()
        End If
        settings("language") = language
    End Sub

    Private Sub MenuStrip1_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles MenuStrip1.ItemClicked

    End Sub
End Class
